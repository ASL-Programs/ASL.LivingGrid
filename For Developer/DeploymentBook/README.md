# DeploymentBook

Bu sənəd WebAdminPanel modulunun yerləşdirilməsi üçün addımları və rejimləri izah edir.

## Hosting rejimləri
- **Standalone (.exe)** – Kestrel üzərində işləyən və Windows servis kimi işə düşə bilən rejim.
- **WebServer (IIS/Apache/Nginx)** – Ənənəvi hostinq, Kestrel reverse proxy ilə və ya IIS in-process.

### Konfiqurasiya
`appsettings.json` faylında `Hosting:Mode` dəyərini dəyişərək rejim seçilir. 
Əlavə olaraq `BackupBeforeSwitch` və `AutoMigrate` parametrləri mövcuddur.

### Rejim dəyişdikdə
1. Cari rejim `hosting_mode.txt` faylında saxlanılır.
2. Yeni rejim seçildikdə əvvəlki rejimlə müqayisə olunur.
3. `BackupBeforeSwitch` aktivdirsə, avtomatik backup `backups/` qovluğuna yaradılır.
4. `AutoMigrate` aktivdirsə, baza miqrasiyası tətbiq olunur.
5. Lazım gələrsə, `MigrationService` ilə bərpa əməliyyatı aparıla bilər.

### IIS/Apache/Nginx yerləşdirilməsi
1. Layihəni `dotnet publish -c Release` əmri ilə yığın.
2. `Hosting:Mode` dəyərini `WebServer` edin.
3. Kestrel portlarını `Kestrel:Endpoints` bölməsində tənzimləyin.
4. Web serverdə reverse proxy qurun (IIS `web.config`, Nginx `proxy_pass`, Apache `ProxyPass`).
5. `X-Forwarded-*` başlıqlarının ötürülməsinə əmin olun, çünki tətbiq `UseForwardedHeaders` istifadə edir.

### Backup və Bərpa
`MigrationService` vasitəsilə `CreateBackupAsync` və `RestoreBackupAsync` metodları mövcuddur. Deployment və rejim dəyişikliyi zamanı bu metodlardan istifadə etmək tövsiyə olunur.

### Gələcək inkişaf
- Avtomatik multi-instance sinxronizasiya
- Bulud və kənar (edge) yerləşdirmə dəstəyi
