# DeploymentBook

Bu sənəd WebAdminPanel modulunun yerləşdirilməsi üçün addımları və rejimləri izah edir.
### Qısa xülasə
Hazırki versiyada dörd yerləşdirmə rejimi sınanmış və istifadəyə hazırdır:
1. **Standalone** (.exe) – Kestrel prosesi kimi çalışır.
2. **WebServer** – IIS, Apache və ya Nginx üzərindən reverse proxy.
3. **Docker** – hazır Dockerfile ilə container kimi.
4. **Serverless Cloud Functions** – `cloudFunctions.json` və `CloudFunctionService`-dən istifadə.

## Hosting rejimləri
- **Standalone (.exe)** – Kestrel üzərində işləyən və Windows servis kimi işə düşə bilən rejim.
- **WebServer (IIS/Apache/Nginx)** – Ənənəvi hostinq, Kestrel reverse proxy ilə və ya IIS in-process.

### Konfiqurasiya
`appsettings.json` faylında `Hosting:Mode` dəyərini dəyişərək rejim seçilir.
Əlavə olaraq `BackupBeforeSwitch` və `AutoMigrate` parametrləri mövcuddur.
HTTPS yönləndirməsini məcburi etmək üçün `Security:RequireHttps` açarını `true`
edə bilərsiniz. Köhnə `ForceHttps` açarı hələ də tanınır, lakin gələcək
versiyalarda çıxarılacaq.
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

### Dağıdılmış və Hibrid Bulud Yerləşdirməsi
`DisasterRecoveryService` və `SyncService` birlikdə multi-cloud mühitlərdə real-time dayanıqlıq təmin edir.
- `disaster_recovery.json` faylında `FailoverNodes` və `BackupPath` parametrləri göstərilir.
- Servis saatda bir backup yaradır və lazım olduqda `/api/failover` sorğusu ilə nodelar arası keçid edir.
- `sync_nodes.json` siyahısı regionlararası konfiqurasiya sinxronizasiyası üçün istifadə olunur.
- Edge şəbəkələr üçün offline rejimdə belə sinxronizasiya imkanı mövcuddur.

### Serverless Mühit üçün Cloud Functions
`CloudFunctionService` serverless ssenariləri üçün `cloudFunctions.json` faylından URL-ləri oxuyur. Konfiqurasiya etmək üçün:
1. WebAdminPanel kök qovluğunda `cloudFunctions.json` yaradın.
2. Faylda hər funksiyanın adını və HTTP endpoint ünvanını `{"FunctionName": "https://your-function-url"}` formatında göstərin. Nümunə üçün `Docs/samples/cloud/README.md` qovluğuna baxın.
3. Müxtəlif mühitlər (dev/stage/prod) üçün ayrıca fayl saxlamaq və yerləşdirmə zamanı uyğun nüsxəni kopyalamaq tövsiyə olunur.
4. Tətbiq işə düşərkən `CloudFunctionService` bu faylı oxuyur və `InvokeAsync` metodu ilə URL-ə JSON sorğu göndərir.
5. Dəyişiklik etdikdən sonra tətbiqi yenidən başladaraq funksiyaların yenilənməsinə əmin olun.
6. Multi-cloud dəstəyi: AWS Lambda, Azure Functions və GCP Cloud Functions kimi istənilən HTTP triggered endpointlər işləyir.

### Default admin hesabının yaradılması
`DEFAULT_ADMIN_EMAIL` və `DEFAULT_ADMIN_PASSWORD` mühit dəyişənlərini daxil etsəniz, WebAdminPanel ilk başladıqda həmin məlumatlarla admin istifadəçi avtomatik yaradılacaq.
Dəyişənlər boş buraxılarsa, heç bir admin hesabı yaradılmır.


### Gələcək inkişaf
- Avtomatik multi-instance sinxronizasiya
- Bulud və kənar (edge) yerləşdirmə dəstəyi
