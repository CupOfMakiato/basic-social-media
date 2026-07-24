resource "neon_project" "this" {
  name                      = "${var.project_name}-${var.environment}"
  org_id                    = var.org_id
  region_id                 = var.region_id
  history_retention_seconds = 21600
  store_password            = "yes"

  branch {
    name = var.environment
  }
}

resource "neon_role" "app" {
  project_id = neon_project.this.id
  branch_id  = neon_project.this.default_branch_id
  name       = "${replace(var.project_name, "-", "_")}_app"
}

resource "neon_database" "app" {
  project_id = neon_project.this.id
  branch_id  = neon_project.this.default_branch_id
  name       = "${replace(var.project_name, "-", "_")}_${replace(var.environment, "-", "_")}"
  owner_name = neon_role.app.name
}
