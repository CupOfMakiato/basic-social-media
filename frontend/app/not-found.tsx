import Link from 'next/link'
import { Home } from 'lucide-react'
import { Button } from '@/components/ui/button'
import {
	Card,
	CardContent,
	CardDescription,
	CardHeader,
	CardTitle,
} from '@/components/ui/card'
import { routes } from '@/config/routes'

export default function NotFoundPage() {
	return (
		<div className='flex min-h-screen items-center justify-center bg-background px-4'>
			<Card className='w-full max-w-md text-center shadow-sm'>
				<CardHeader>
					<p className='text-5xl font-semibold' aria-hidden='true'>
						404
					</p>
					<CardTitle className='text-2xl'>Page not found</CardTitle>
					<CardDescription>
						The page may have moved or no longer exists.
					</CardDescription>
				</CardHeader>
				<CardContent>
					<Button asChild>
						<Link href={routes.landing}>
							<Home data-icon='inline-start' />
							Go home
						</Link>
					</Button>
				</CardContent>
			</Card>
		</div>
	)
}
