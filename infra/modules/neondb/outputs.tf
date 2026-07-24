output "project_id" {
  value = neon_project.this.id
}

output "database_connection_string" {
  value     = "Host=${neon_project.this.database_host};Database=${neon_database.app.name};Username=${neon_role.app.name};Password=${neon_role.app.password};SSL Mode=Require"
  sensitive = true
}
