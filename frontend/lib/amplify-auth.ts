import { Amplify } from 'aws-amplify'

type AmplifyAuthConfig = {
	userPoolId: string
	userPoolClientId: string
}

let configuredKey: string | undefined

export function configureAmplifyAuth({
	userPoolId,
	userPoolClientId,
}: AmplifyAuthConfig) {
	const key = `${userPoolId}:${userPoolClientId}`
	if (configuredKey === key) return

	Amplify.configure({
		Auth: {
			Cognito: {
				userPoolId,
				userPoolClientId,
				loginWith: { email: true },
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
	})

	configuredKey = key
}

export function isAmplifyAuthConfigured() {
	return configuredKey !== undefined
}
