'use client'

import { useState, type FormEvent } from 'react'
import Link from 'next/link'
import { Button } from '@/components/ui/button'
import {
	Card,
	CardContent,
	CardDescription,
	CardHeader,
	CardTitle,
} from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { routes } from '@/config/routes'
import { useLogin } from '@/hooks/auth/use-login'
import { isAmplifyAuthConfigured } from '@/lib/amplify-auth'

type LoginFormProps = {
	confirmed: boolean
}

export function LoginForm({ confirmed }: LoginFormProps) {
	const [email, setEmail] = useState('')
	const [password, setPassword] = useState('')
	const [confirmationCode, setConfirmationCode] = useState('')
	const [needsCode, setNeedsCode] = useState(false)
	const [stepError, setStepError] = useState<string>()
	const { login, confirmLogin, isLoading, error, clearError } = useLogin()

	const configReady = isAmplifyAuthConfigured()

	async function handleLogin(event: FormEvent<HTMLFormElement>) {
		event.preventDefault()
		if (!configReady) return

		clearError()
		setStepError(undefined)

		try {
			const result = await login({ email, password })

			if (result.isSignedIn) return

			if (
				result.nextStep.signInStep === 'CONFIRM_SIGN_IN_WITH_EMAIL_CODE' ||
				result.nextStep.signInStep === 'CONFIRM_SIGN_IN_WITH_SMS_CODE' ||
				result.nextStep.signInStep === 'CONFIRM_SIGN_IN_WITH_TOTP_CODE'
			) {
				setNeedsCode(true)
				return
			}

			setStepError(
				'This account requires an additional sign-in step that is not available yet.'
			)
		} catch {}
	}

	async function handleConfirmation(event: FormEvent<HTMLFormElement>) {
		event.preventDefault()
		if (!configReady) return

		clearError()
		setStepError(undefined)

		try {
			const result = await confirmLogin(confirmationCode)

			if (!result.isSignedIn) {
				setStepError('Another sign-in step is required.')
			}
		} catch {}
	}

	return (
		<Card className='w-full max-w-sm shadow-sm'>
			<CardHeader>
				<CardTitle className='text-2xl'>BasicSocialMedia</CardTitle>
				<CardDescription>
					{needsCode
						? 'Enter the verification code for your account'
						: 'Sign in to your account'}
				</CardDescription>
			</CardHeader>
			<CardContent>
				<form
					className='space-y-4'
					onSubmit={needsCode ? handleConfirmation : handleLogin}
				>
					{confirmed && !needsCode ? (
						<p className='text-sm text-foreground' role='status'>
							Email confirmed. You can sign in now.
						</p>
					) : null}

					{needsCode ? (
						<div className='space-y-2'>
							<Label htmlFor='confirmation-code'>Verification code</Label>
							<Input
								id='confirmation-code'
								inputMode='numeric'
								autoComplete='one-time-code'
								value={confirmationCode}
								onChange={event => setConfirmationCode(event.target.value)}
								required
							/>
						</div>
					) : (
						<>
							<div className='space-y-2'>
								<Label htmlFor='email'>Email</Label>
								<Input
									id='email'
									type='email'
									autoComplete='email'
									value={email}
									onChange={event => setEmail(event.target.value)}
									required
								/>
							</div>
							<div className='space-y-2'>
								<Label htmlFor='password'>Password</Label>
								<Input
									id='password'
									type='password'
									autoComplete='current-password'
									value={password}
									onChange={event => setPassword(event.target.value)}
									required
								/>
							</div>
						</>
					)}

					{!configReady ? (
						<p className='text-sm text-destructive' role='alert'>
							Cognito is not configured for this environment.
						</p>
					) : null}
					{error ? (
						<p className='text-sm text-destructive' role='alert'>
							{error}
						</p>
					) : null}
					{stepError ? (
						<p className='text-sm text-destructive' role='alert'>
							{stepError}
						</p>
					) : null}

					<Button
						className='w-full'
						type='submit'
						disabled={!configReady || isLoading}
					>
						{isLoading ? 'Please wait...' : needsCode ? 'Verify' : 'Sign in'}
					</Button>

					{!needsCode ? (
						<p className='text-center text-sm text-muted-foreground'>
							No account?{' '}
							<Link
								className='font-medium text-foreground underline-offset-4 hover:underline'
								href={routes.signUp}
							>
								Sign up
							</Link>
						</p>
					) : null}
				</form>
			</CardContent>
		</Card>
	)
}
