'use client'

import { useEffect } from 'react'
import { configureAmplifyAuth } from '@/lib/amplify-auth'
import { useAuthStore } from '@/stores/auth-store'

type AmplifyProviderProps = {
	children: React.ReactNode
	userPoolId?: string
	userPoolClientId?: string
}

export function AmplifyProvider({
	children,
	userPoolId,
	userPoolClientId,
}: AmplifyProviderProps) {
	const refreshAuth = useAuthStore(state => state.refreshAuth)

	if (userPoolId && userPoolClientId) {
		configureAmplifyAuth({ userPoolId, userPoolClientId })
	}

	useEffect(() => {
		void refreshAuth()
	}, [refreshAuth])

	return children
}
