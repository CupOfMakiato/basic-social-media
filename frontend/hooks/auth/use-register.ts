'use client'

import { useCallback, useEffect } from 'react'
import { useRouter } from 'next/navigation'
import { routes } from '@/config/routes'
import { useAuthStore } from '@/stores/auth-store'
import type { ConfirmRegistrationInput, RegisterInput } from '@/types/auth'

export function useRegister() {
	const router = useRouter()
	const signUp = useAuthStore(state => state.signUp)
	const confirmSignUp = useAuthStore(state => state.confirmSignUp)
	const signIn = useAuthStore(state => state.signIn)
	const isLoading = useAuthStore(state => state.isLoading)
	const error = useAuthStore(state => state.error)
	const clearError = useAuthStore(state => state.clearError)

	useEffect(() => {
		clearError()
	}, [clearError])

	const finishRegistration = useCallback(
		async (input: RegisterInput) => {
			const result = await signIn(input)

			if (result.isSignedIn) {
				router.push(routes.landing)
				router.refresh()
				return
			}

			router.push(routes.signIn)
		},
		[router, signIn]
	)

	const register = useCallback(
		async (input: RegisterInput) => {
			const result = await signUp(input)

			if (result.isSignUpComplete) {
				await finishRegistration(input)
			}

			return result
		},
		[finishRegistration, signUp]
	)

	const confirmRegistration = useCallback(
		async (input: ConfirmRegistrationInput) => {
			await confirmSignUp(input)
			await finishRegistration(input)
		},
		[confirmSignUp, finishRegistration]
	)

	return {
		register,
		confirmRegistration,
		isLoading,
		error,
		clearError,
	}
}
