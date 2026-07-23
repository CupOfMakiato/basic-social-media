const apiBaseUrl = (
	process.env.API_BASE_URL ?? 'http://localhost:5148'
).replace(/\/+$/, '')

export async function POST(request: Request) {
	const authorization = request.headers.get('authorization')
	if (!authorization?.startsWith('Bearer ')) {
		return Response.json(
			{ message: 'Cognito token is required.' },
			{ status: 401 }
		)
	}

	const apiResponse = await fetch(`${apiBaseUrl}/api/Auth/cognito`, {
		method: 'POST',
		headers: { Authorization: authorization },
		cache: 'no-store',
	}).catch(() => null)

	if (!apiResponse) {
		return Response.json(
			{ message: 'The authentication service is unavailable.' },
			{ status: 502 }
		)
	}

	const response = new Response(await apiResponse.arrayBuffer(), {
		status: apiResponse.status,
		headers: {
			'Content-Type':
				apiResponse.headers.get('content-type') ?? 'application/json',
		},
	})
	const headers = apiResponse.headers as Headers & {
		getSetCookie?: () => string[]
	}

	for (const cookie of headers.getSetCookie?.() ?? []) {
		response.headers.append('Set-Cookie', cookie)
	}

	return response
}
