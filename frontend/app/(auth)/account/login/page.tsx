import { LoginForm } from './login-form'

export default async function LoginPage({
	searchParams,
}: {
	searchParams: Promise<{ confirmed?: string }>
}) {
	const { confirmed } = await searchParams

	return (
		<div className='flex min-h-screen items-center justify-center bg-background px-4'>
			<LoginForm confirmed={confirmed === '1'} />
		</div>
	)
}
