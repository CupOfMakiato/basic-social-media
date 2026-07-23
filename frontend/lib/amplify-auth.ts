import { Amplify } from 'aws-amplify'

type AmplifyAuthConfig = {
	userPoolId: string
	userPoolClientId: string
	hostedUiUrl?: string
}

let configuredKey: string | undefined
let oauthConfigured = false

export function configureAmplifyAuth({
	userPoolId,
	userPoolClientId,
	hostedUiUrl,
}: AmplifyAuthConfig) {
	const origin = typeof window === 'undefined' ? undefined : window.location.origin
	const domain = getCognitoDomain(hostedUiUrl)
	const key = `${userPoolId}:${userPoolClientId}:${domain ?? ''}:${origin ?? ''}`
	if (configuredKey === key) return

	const oauth =
		domain && origin
			? {
					oauth: {
						domain,
						scopes: ['email', 'openid', 'profile'],
						redirectSignIn: [`${origin}/auth/callback`],
						redirectSignOut: [origin],
						responseType: 'code' as const,
					},
				}
			: {}

	Amplify.configure(
		{
			Auth: {
				Cognito: {
					userPoolId,
					userPoolClientId,
					loginWith: { email: true, ...oauth },
					signUpVerificationMethod: 'code',
					userAttributes: {
						email: { required: true },
					},
					passwordFormat: {
						minLength: 8,
						requireLowercase: true,
						requireUppercase: true,
						requireNumbers: true,
						requireSpecialCharacters: false,
					},
				},
			},
		},
		{ ssr: true }
	)

	configuredKey = key
	oauthConfigured = Boolean(oauth.oauth)
}

export function isAmplifyAuthConfigured() {
	return configuredKey !== undefined
}

export function isAmplifyOAuthConfigured() {
	return oauthConfigured
}

function getCognitoDomain(hostedUiUrl?: string) {
	if (!hostedUiUrl) return undefined

	try {
		return new URL(hostedUiUrl).host
	} catch {
		return hostedUiUrl.replace(/^https?:\/\//, '').replace(/\/+$/, '')
	}
}
