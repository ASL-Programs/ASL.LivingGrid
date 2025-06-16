# SecurityBook

Bu sənəd WebAdminPanel modulunda təhlükəsizlik və parametrlərin idarə olunmasını izah edir.

## Qısa xülasə
- **General Settings** səhifəsində tema, API və autentifikasiya endpointləri bir yerdə qruplanır.
- **Security Settings** bölməsində 2FA və Google SSO açarları idarə edilir.
- Bütün dəyişikliklər `AuditService` vasitəsilə qeyd olunur və əvvəlki vəziyyətə geri qaytarmaq imkanı var.
- Həssas məlumatlar `TpmHsmSecretStorageService` ilə şifrələnərək saxlanılır.

## Niyə yaradılıb?
Sistemin təhlükəsizliyini mərkəzləşdirilmiş şəkildə tənzimləmək və konfiqurasiya dəyişikliklərinə nəzarət etmək üçün.

## Nəyə xidmət edir?
İstifadəçilərə əsas təhlükəsizlik parametrlərini UI üzərindən rahat dəyişməyə və audit izləməyə imkan verir.

## İstifadə qaydası və idarəetmə prinsipləri
1. Menyudan **Settings → General** və ya **Settings → Security** seçin.
2. Lazımi dəyərləri dəyişib **Save** düyməsini sıxın.
3. Hər dəyişiklik audit logda qeydə alınır və lazım olduqda `Rollback` funksiyası ilə əvvəlki dəyərə qayıtmaq mümkündür.

## Texniki və biznes üstünlükləri
- Dinamik 2FA/SSO tənzimləməsi.
- HSM/TPM əsaslı gizli saxlama modulu.
- Konfiqurasiyaların tam izlənməsi və bərpası.

## Gələcək inkişaf yolları və risklər
- SAML və digər identifikasiya protokollarının əlavə edilməsi.
- Parol siyasətlərinin daha sərt tətbiqi və avtomatik rotasiya.

## Yeni xüsusiyyətlər və konfiqurasiya
1. **JIT səlahiyyət artırılması** – `Security:EnableJitPrivilegeElevation` açarını `true` edin və istifadəçi üçün müddət təyin edin.
2. **Gizli açarların rotasiyası** – `Security:SecretRotationDays` dəyəri günlərlə rotasiya intervalını göstərir. `SecurityService.RotateSecretsAsync` fon xidməti ilə avtomatlaşdırılır.
3. **Tenant üzrə siyasət** – `Security:EnforcePerTenantPolicy` açarı aktiv olduqda hər tenant üçün `Security:Policy:{tenantId}` konfiqurasiyası tətbiq edilir.
4. **Xarici identifikasiya provayderləri** – `Security:Oidc:*`, `Security:OAuth2:*`, `Security:Saml:*` və `Security:Ldap:*` parametrlərini dolduraraq qoşun.
5. **Parol vaxtı və rotasiyası** – `Security:PasswordExpiryDays` dəyəri bitdikdə istifadəçi kilidlənir, yeni parol tələb olunur.
6. **Wireframe önizləmə tokeni** – `Security:PreviewSecret` məcburi dəyərdir. Bu secret ilə imzalanmış `token` parametri olmadan `/wireframes/preview/{id}` ünvanına giriş verilmir.
7. **Gizli açarın təyin olunması** – `Security:PreviewSecret` dəyərini `appsettings.json`-da saxlamayın. Açarı `Security__PreviewSecret` mühit dəyişəni və ya `dotnet user-secrets` vasitəsilə təyin edin. TPM/HSM əsaslı `TpmHsmSecretStorageService` ilə qorumağı unutmayın.

