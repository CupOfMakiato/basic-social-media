'use client'

import 'aws-amplify/auth/enable-oauth-listener'

import { useEffect, useState } from 'react'
import { useRouter } from 'next/navigation'
import { Hub } from 'aws-amplify/utils'
import { routes } from '@/config/routes'
import { createAppSession } from '@/lib/auth-session'

export default function AuthCallbackPage() {
	const router = useRouter()
	const [error, setError] = useState<string>()

	useEffect(() => {
		let active = true
		let finished = false

		async function finish() {
			if (finished) return
			finished = true

			try {
				await createAppSession()
				if (!active) return

				router.replace(routes.landing)
				router.refresh()
			} catch (error) {
				finished = false
				if (active) {
					setError(error instanceof Error ? error.message : 'Sign in failed.')
				}
			}
		}

		const unsubscribe = Hub.listen('auth', ({ payload }) => {
			if (payload.event === 'signInWithRedirect') void finish()
			if (payload.event === 'signInWithRedirect_failure') {
				setError('Sign in failed.')
			}
		})
		const fallback = window.setTimeout(() => void finish(), 1000)

		return () => {
			active = false
			window.clearTimeout(fallback)
			unsubscribe()
		}
	}, [router])

	return (
		<main className='flex min-h-screen items-center justify-center px-4 text-center'>
			<p className={error ? 'text-sm text-destructive' : 'text-sm'}>
				{error ?? 'Signing you in...'}
			</p>
		</main>
	)
}
