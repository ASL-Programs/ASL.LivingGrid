# DeploymentBook

Bu sənəd WebAdminPanel modulunun yerləşdirilməsi üçün addımları və rejimləri izah edir.

## Hosting rejimləri
- **Standalone (.exe)** – Kestrel üzərində işləyən və Windows servis kimi işə düşə bilən rejim.
- **WebServer (IIS/Apache/Nginx)** – Ənənəvi hostinq, Kestrel reverse proxy ilə və ya IIS in-process.

### Konfiqurasiya
`appsettings.json` faylında `Hosting:Mode` dəyərini dəyişərək rejim seçilir.
Əlavə olaraq `BackupBeforeSwitch` və `AutoMigrate` parametrləri mövcuddur.
Komanda sətrindən `--standalone` və ya `--hosted` parametrini əlavə etməklə rejim dərhal dəyişdirilə bilər.

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

### Docker ilə yerləşdirilməsi
1. `WebAdminPanel` qovluğunda təqdim olunan `Dockerfile` istifadə edin.
2. `docker build -t livinggrid-admin .` əmri ilə image yaradın.
3. `docker run -d -p 8080:8080 --name livinggrid-admin livinggrid-admin` əmrini işlədirək tətbiqi container-də başladın.
4. `Hosting:Mode` dəyəri avtomatik `Standalone` olaraq qalır və Kestrel 8080 portunu dinləyir.
5. İstəyə uyğun `docker-compose` və ya Kubernetes faylı hazırlamaq olar.

### Backup və Bərpa
`MigrationService` vasitəsilə `CreateBackupAsync` və `RestoreBackupAsync` metodları mövcuddur. Deployment və rejim dəyişikliyi zamanı bu metodlardan istifadə etmək tövsiyə olunur.

### Multi-instance Sinxronizasiya
`SyncService` periodik olaraq `sync_nodes.json` faylında göstərilən digər instansiyalara `/api/sync/ping` göndərir. Bu mexanizm konfiqurasiya və dil dəyişikliklərini bütün nodelar arasında bölüşməyə imkan verir.

### Kənar və Bulud Yayımı
`SyncService` həm on-prem, həm də bulud və edge instansiyalarını dəstəkləyir. Docker image və ya klassik hostinq istifadə edilə bilər. Fərqli mühitlər arasında avtomatik backup və sinxronizasiya təmin edilir.

### Serverless Mühit üçün Cloud Functions
`CloudFunctionService` serverless ssenariləri üçün `cloudFunctions.json` faylından URL-ləri oxuyur. Konfiqurasiya etmək üçün:
1. WebAdminPanel kök qovluğunda `cloudFunctions.json` yaradın.
2. Faylda hər funksiyanın adını və HTTP endpoint ünvanını `{"FunctionName": "https://example.com/api"}` formatında göstərin.
3. Müxtəlif mühitlər (dev/stage/prod) üçün ayrıca fayl saxlamaq və yerləşdirmə zamanı uyğun nüsxəni kopyalamaq tövsiyə olunur.
4. Tətbiq işə düşərkən `CloudFunctionService` bu faylı oxuyur və `InvokeAsync` metodu ilə URL-ə JSON sorğu göndərir.
5. Dəyişiklik etdikdən sonra tətbiqi yenidən başladaraq funksiyaların yenilənməsinə əmin olun.


### Gələcək inkişaf
- Avtomatik multi-instance sinxronizasiya
- Bulud və kənar (edge) yerləşdirmə dəstəyi
