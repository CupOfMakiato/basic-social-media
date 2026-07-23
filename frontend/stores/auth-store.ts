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

function getErrorMessage(error: unknown) {
	if (!(error instanceof Error)) {
		return 'Authentication could not be completed. Please try again.'
	}

	return process.env.NODE_ENV === 'development'
		? `${error.name}: ${error.message}`
		: error.message
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
