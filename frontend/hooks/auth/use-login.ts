'use client'

import { useCallback, useEffect } from 'react'
import { useRouter } from 'next/navigation'
import { routes } from '@/config/routes'
import { useAuthStore } from '@/stores/auth-store'
import type { LoginInput } from '@/types/auth'

export function useLogin() {
	const router = useRouter()
	const signIn = useAuthStore(state => state.signIn)
	const confirmSignIn = useAuthStore(state => state.confirmSignIn)
	const isLoading = useAuthStore(state => state.isLoading)
	const error = useAuthStore(state => state.error)
	const clearError = useAuthStore(state => state.clearError)

	useEffect(() => {
		clearError()
	}, [clearError])

	const login = useCallback(
		async (input: LoginInput) => {
			const result = await signIn(input)

			if (result.isSignedIn) {
				router.push(routes.landing)
				router.refresh()
			}

			return result
		},
		[router, signIn]
	)

	const confirmLogin = useCallback(
		async (code: string) => {
			const result = await confirmSignIn(code)

			if (result.isSignedIn) {
				router.push(routes.landing)
				router.refresh()
			}

			return result
		},
		[confirmSignIn, router]
	)

	return { login, confirmLogin, isLoading, error, clearError }
}
