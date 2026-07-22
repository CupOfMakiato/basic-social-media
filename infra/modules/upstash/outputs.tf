output "database_id" {
  value = upstash_redis_database.this.database_id
}

output "endpoint" {
  value = upstash_redis_database.this.endpoint
}

output "connection_string" {
  value     = "rediss://default:${urlencode(upstash_redis_database.this.password)}@${upstash_redis_database.this.endpoint}:${upstash_redis_database.this.port}"
  sensitive = true
}
