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
import { useRegister } from '@/hooks/auth/use-register'
import { isAmplifyAuthConfigured } from '@/lib/amplify-auth'

export function RegisterForm() {
	const [email, setEmail] = useState('')
	const [password, setPassword] = useState('')
	const [confirmationCode, setConfirmationCode] = useState('')
	const [needsConfirmation, setNeedsConfirmation] = useState(false)
	const { register, confirmRegistration, isLoading, error, clearError } =
		useRegister()

	const configReady = isAmplifyAuthConfigured()

	async function handleSignUp(event: FormEvent<HTMLFormElement>) {
		event.preventDefault()
		if (!configReady) return

		clearError()

		try {
			const result = await register({ email, password })

			if (!result.isSignUpComplete) setNeedsConfirmation(true)
		} catch {}
	}

	async function handleConfirmation(event: FormEvent<HTMLFormElement>) {
		event.preventDefault()
		if (!configReady) return

		clearError()

		try {
			await confirmRegistration({ email, code: confirmationCode })
		} catch {}
	}

	return (
		<Card className='w-full max-w-sm shadow-sm'>
			<CardHeader>
				<CardTitle className='text-2xl'>Create account</CardTitle>
				<CardDescription>
					{needsConfirmation
						? `We sent a verification code to ${email}`
						: 'Sign up with your email'}
				</CardDescription>
			</CardHeader>
			<CardContent>
				<form
					className='space-y-4'
					onSubmit={needsConfirmation ? handleConfirmation : handleSignUp}
				>
					{needsConfirmation ? (
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
									autoComplete='new-password'
									minLength={8}
									value={password}
									onChange={event => setPassword(event.target.value)}
									required
								/>
								<p className='text-xs text-muted-foreground'>
									Use at least 8 characters with uppercase, lowercase, and a
									number.
								</p>
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

					<Button
						className='w-full'
						type='submit'
						disabled={!configReady || isLoading}
					>
						{isLoading
							? 'Please wait...'
							: needsConfirmation
								? 'Confirm email'
								: 'Create account'}
					</Button>

					{!needsConfirmation ? (
						<p className='text-center text-sm text-muted-foreground'>
							Already registered?{' '}
							<Link
								className='font-medium text-foreground underline-offset-4 hover:underline'
								href={routes.signIn}
							>
								Sign in
							</Link>
						</p>
					) : null}
				</form>
			</CardContent>
		</Card>
	)
}
