import Link from 'next/link'
import { Button } from '@/components/ui/button'
import {
	NavigationMenu,
	NavigationMenuList,
} from '@/components/ui/navigation-menu'
import { Avatar } from '@/components/ui/avatar'
import { routes } from '@/config/routes'

export default function LandingPage() {
	return (
		<div className='min-h-screen bg-background text-foreground'>
			<header className='flex h-14 items-center justify-between border-b px-4 sm:px-6'>
				<Avatar className='h-8 w-8'></Avatar>
				<NavigationMenu>
					<NavigationMenuList />
				</NavigationMenu>

				<div className='flex items-center gap-2'>
					<Button asChild variant='ghost'>
						<Link href={routes.signIn}>Sign in</Link>
					</Button>
					<Button asChild>
						<Link href={routes.signUp}>Sign up</Link>
					</Button>
				</div>
			</header>

			<main className='flex min-h-[calc(100vh-3.5rem)] items-center justify-center px-4 text-center'>
				<h1 className='text-4xl font-semibold sm:text-5xl'>test</h1>
			</main>
		</div>
	)
}
