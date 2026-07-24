# Infrastructure

This stack creates one environment at a time. Use a separate Terraform state
for development and production.

Required Doppler secrets for the new resources:

- `NEON_API_KEY`
- `NEON_ORG_ID`
- `CLOUDFLARE_API_TOKEN`
- `R2_ACCOUNT_ID`
- `R2_BUCKET_NAME`
- `TEST_ADMIN_EMAIL`
- `TEST_ADMIN_PASSWORD`
- `TEST_USER_EMAIL`
- `TEST_USER_PASSWORD`

The Cloudflare token needs `Workers R2 Storage Write`. Give development and
production different `R2_BUCKET_NAME` values so separate states do not manage
the same bucket.

`TF_NEON_REGION` defaults to `aws-ap-southeast-1`, and `R2_LOCATION` defaults
to `apac`.

Terraform writes these generated Neon values back to the selected Doppler
config:

- `CONNECTIONSTRINGS_DEFAULTCONNECTION`

Apply development using the existing `default` workspace:

```powershell
$env:TF_VAR_DOPPLER_TOKEN = "<doppler-personal-token>"
doppler run --preserve-env="TF_VAR_DOPPLER_TOKEN" -- terraform init
doppler run --preserve-env="TF_VAR_DOPPLER_TOKEN" -- terraform workspace select default
doppler run --preserve-env="TF_VAR_DOPPLER_TOKEN" -- terraform apply -var="doppler_config=dev"
```

Create production in its own state:

```powershell
doppler run --preserve-env="TF_VAR_DOPPLER_TOKEN" -- terraform workspace new production
doppler run --preserve-env="TF_VAR_DOPPLER_TOKEN" -- terraform apply -var="doppler_config=production"
```

Set `TF_ENVIRONMENT=production` in the production Doppler config before the
production apply. Use an encrypted remote backend before team or production
use because Terraform state contains generated database and test-user
credentials.

The token must be able to write secrets because Terraform saves the generated
Neon connection string as `CONNECTIONSTRINGS_DEFAULTCONNECTION`. A read-only
service token can read the config but cannot complete this resource.

The Cognito test administrator and normal user receive permanent passwords and
join the `Admins` and `Users` groups respectively. The application creates or
links each `"User"` table row on first Cognito login and synchronizes its role
from the validated Cognito group.

The frontend uses OpenNext on Cloudflare Workers, so this stack does not create
a Pages project. Add DNS after choosing the production hostname and Worker
custom-domain target.

note
```powershell
$connection = doppler secrets get CONNECTIONSTRINGS_DEFAULTCONNECTION --plain

dotnet ef database update --project BasicSocialMedia.Infrastructure --startup-project BasicSocialMedia.API --connection "$connection"

Remove-Variable connection
```