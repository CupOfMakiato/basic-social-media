import type { Metadata } from 'next'
import { JetBrains_Mono, Merriweather, Outfit } from 'next/font/google'
import { AmplifyProvider } from '@/components/providers/amplify-provider'
import './globals.css'

const outfit = Outfit({
	variable: '--font-outfit',
	subsets: ['latin'],
})

const jetbrainsMono = JetBrains_Mono({
	variable: '--font-jetbrains-mono',
	subsets: ['latin'],
})

const merriweather = Merriweather({
	variable: '--font-merriweather',
	subsets: ['latin'],
})

export const metadata: Metadata = {
	title: 'BasicSocialMedia',
	description: 'BasicSocialMedia frontend',
}

export default function RootLayout({
	children,
}: Readonly<{
	children: React.ReactNode
}>) {
	return (
		<html
			lang='en'
			className={`${outfit.variable} ${jetbrainsMono.variable} ${merriweather.variable} antialiased`}
		>
			<body className='min-h-screen'>
				<AmplifyProvider
					hostedUiUrl={process.env.COGNITO_HOSTED_UI_URL}
					userPoolClientId={process.env.COGNITO_CLIENT_ID}
					userPoolId={process.env.COGNITO_USER_POOL_ID}
				>
					{children}
				</AmplifyProvider>
			</body>
		</html>
	)
}
