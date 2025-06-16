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

