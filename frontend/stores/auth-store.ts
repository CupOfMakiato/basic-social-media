'use client'

import { create } from 'zustand'
import {
	confirmSignIn as amplifyConfirmSignIn,
	confirmSignUp as amplifyConfirmSignUp,
	getCurrentUser,
	signIn as amplifySignIn,
	signOut as amplifySignOut,
	signUp as amplifySignUp,
} from 'aws-amplify/auth'
import { createAppSession } from '@/lib/auth-session'
import type { AuthStore } from '@/types/auth'

const cognitoErrorMessages: Record<string, string> = {
	AliasExistsException:
		'Sign up could not be completed. Check your details and try again.',
	CodeMismatchException: 'The verification code is invalid or expired.',
	ExpiredCodeException: 'The verification code is invalid or expired.',
	InvalidPasswordException:
		'Sign up could not be completed. Check your details and try again.',
	InvalidParameterException:
		'Authentication could not be completed. Please try again.',
	LimitExceededException: 'Too many attempts. Please wait and try again.',
	NotAuthorizedException: 'Email or password is incorrect.',
	PasswordResetRequiredException:
		'Authentication could not be completed. Please reset your password.',
	TooManyFailedAttemptsException: 'Too many attempts. Please wait and try again.',
	TooManyRequestsException: 'Too many attempts. Please wait and try again.',
	UserNotConfirmedException:
		'Authentication could not be completed. Please verify your account.',
	UserNotFoundException: 'Email or password is incorrect.',
	UsernameExistsException:
		'Sign up could not be completed. Check your details and try again.',
}

function getErrorMessage(error: unknown) {
	if (!(error instanceof Error)) {
		return 'Authentication could not be completed. Please try again.'
	}

	if (process.env.NODE_ENV === 'development') {
		return `${error.name}: ${error.message}`
	}

	return (
		cognitoErrorMessages[error.name] ??
		'Authentication could not be completed. Please try again.'
	)
}

async function completeSignIn() {
	await createAppSession()
	return getCurrentUser()
}

export const useAuthStore = create<AuthStore>()(set => ({
	user: null,
	isAuthenticated: false,
	isLoading: true,
	error: null,

	signIn: async ({ email, password }) => {
		set({
			user: null,
			isAuthenticated: false,
			isLoading: true,
			error: null,
		})

		try {
			await amplifySignOut().catch(() => undefined)
			const result = await amplifySignIn({ username: email, password })

			if (result.isSignedIn) {
				const user = await completeSignIn()
				set({ user, isAuthenticated: true })
			}

			return result
		} catch (error) {
			set({
				user: null,
				isAuthenticated: false,
				error: getErrorMessage(error),
			})
			throw error
		} finally {
			set({ isLoading: false })
		}
	},

	confirmSignIn: async code => {
		set({ isLoading: true, error: null })

		try {
			const result = await amplifyConfirmSignIn({ challengeResponse: code })

			if (result.isSignedIn) {
				const user = await completeSignIn()
				set({ user, isAuthenticated: true })
			}

			return result
		} catch (error) {
			set({
				user: null,
				isAuthenticated: false,
				error: getErrorMessage(error),
			})
			throw error
		} finally {
			set({ isLoading: false })
		}
	},

	signUp: async ({ email, password }) => {
		set({ isLoading: true, error: null })

		try {
			return await amplifySignUp({
				username: email,
				password,
				options: { userAttributes: { email } },
			})
		} catch (error) {
			set({ error: getErrorMessage(error) })
			throw error
		} finally {
			set({ isLoading: false })
		}
	},

	confirmSignUp: async ({ email, code }) => {
		set({ isLoading: true, error: null })

		try {
			return await amplifyConfirmSignUp({
				username: email,
				confirmationCode: code,
			})
		} catch (error) {
			set({ error: getErrorMessage(error) })
			throw error
		} finally {
			set({ isLoading: false })
		}
	},

	refreshAuth: async () => {
		try {
			const user = await getCurrentUser()
			set({ user, isAuthenticated: true, isLoading: false, error: null })
		} catch {
			set({
				user: null,
				isAuthenticated: false,
				isLoading: false,
				error: null,
			})
		}
	},

	clearError: () => set({ error: null }),
}))
