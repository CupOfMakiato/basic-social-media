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
	const isLoading = useAuthStore(state => state.isLoading)
	const error = useAuthStore(state => state.error)
	const clearError = useAuthStore(state => state.clearError)

	useEffect(() => {
		clearError()
	}, [clearError])

	const register = useCallback(
		async (input: RegisterInput) => {
			const result = await signUp(input)

			if (result.isSignUpComplete) {
				router.push(`${routes.signIn}?confirmed=1`)
			}

			return result
		},
		[router, signUp]
	)

	const confirmRegistration = useCallback(
		async (input: ConfirmRegistrationInput) => {
			await confirmSignUp(input)
			router.push(`${routes.signIn}?confirmed=1`)
		},
		[confirmSignUp, router]
	)

	return {
		register,
		confirmRegistration,
		isLoading,
		error,
		clearError,
	}
}
