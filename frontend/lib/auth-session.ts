import { fetchAuthSession } from 'aws-amplify/auth'

export async function createAppSession() {
	const session = await fetchAuthSession()
	const idToken = session.tokens?.idToken?.toString()

	if (!idToken) {
		throw new Error('Cognito did not return an ID token.')
	}

	const response = await fetch('/api/auth/cognito', {
		method: 'POST',
		headers: { Authorization: `Bearer ${idToken}` },
	})

	if (!response.ok) {
		throw new Error('The application session could not be created.')
	}
}
