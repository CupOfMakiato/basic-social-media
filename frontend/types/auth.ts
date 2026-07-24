import type {
	AuthUser,
	ConfirmSignUpOutput,
	SignInOutput,
	SignUpOutput,
} from 'aws-amplify/auth'

export type LoginInput = {
	email: string
	password: string
}

export type RegisterInput = LoginInput

export type ConfirmRegistrationInput = RegisterInput & {
	code: string
}

export type AuthStore = {
	user: AuthUser | null
	isAuthenticated: boolean
	isLoading: boolean
	error: string | null
	signIn: (input: LoginInput) => Promise<SignInOutput>
	confirmSignIn: (code: string) => Promise<SignInOutput>
	signUp: (input: RegisterInput) => Promise<SignUpOutput>
	confirmSignUp: (
		input: ConfirmRegistrationInput
	) => Promise<ConfirmSignUpOutput>
	refreshAuth: () => Promise<void>
	clearError: () => void
}
