'use client'

import Link from 'next/link'
import { AlertTriangle, Home, RotateCcw } from 'lucide-react'
import { Button } from '@/components/ui/button'
import {
	Card,
	CardContent,
	CardDescription,
	CardHeader,
	CardTitle,
} from '@/components/ui/card'
import { routes } from '@/config/routes'

export default function ErrorPage({
	error,
	reset,
}: {
	error: Error & { digest?: string }
	reset: () => void
}) {
	return (
		<div className='flex min-h-screen items-center justify-center bg-background px-4'>
			<Card className='w-full max-w-md shadow-sm'>
				<CardHeader className='text-center'>
					<AlertTriangle
						className='mx-auto mb-2 size-10 text-destructive'
						aria-hidden='true'
					/>
					<CardTitle className='text-2xl'>Something went wrong</CardTitle>
					<CardDescription>
						We could not finish loading this page.
					</CardDescription>
				</CardHeader>
				<CardContent className='space-y-4'>
					{process.env.NODE_ENV === 'development' ? (
						<p className='text-sm text-destructive' role='alert'>
							{error.message}
						</p>
					) : null}
					<div className='flex justify-center gap-2'>
						<Button type='button' variant='outline' onClick={reset}>
							<RotateCcw data-icon='inline-start' />
							Try again
						</Button>
						<Button asChild>
							<Link href={routes.landing}>
								<Home data-icon='inline-start' />
								Go home
							</Link>
						</Button>
					</div>
				</CardContent>
			</Card>
		</div>
	)
}
