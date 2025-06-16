# PluginBook

Bu sənəd plugin marketplace səhifəsinin yaradılması və istifadəsini izah edir.

## Niyə yaradılıb?
- Sistemə əlavə funksionallıq gətirən pluginləri mərkəzləşmiş şəkildə yayımlamaq.
- İstifadəçilərə və developer komandaya versiya və konfiqurasiya idarəsini asanlaşdırmaq.

## Nəyə xidmət edir?
- Marketplace vasitəsilə pluginləri yükləmək və yeniləmək.
- UI üzərindən hər plugin üçün konfiqurasiyanı tənzimləmək və aktivləşdirmək.

## İstifadə qaydası və idarəetmə prinsipləri
1. `Plugin Marketplace` menyu bölməsindən mövcud pluginləri siyahı şəklində görmək mümkündür.
2. "Install" düyməsi seçilmiş pluginin JSON paketini serverə yükləyir və `PluginService` vasitəsilə qeydiyyata alır.
3. "Export" funksiyası mövcud pluginin konfiqurasiyasını JSON kimi yükləməyə imkan verir.
4. Hər dəyişiklik `plugins.json` faylında saxlanılır və gələcəkdə avtomatik yüklənir.

## Texniki və biznes üstünlükləri
- **Modulluq:** Yeni imkanları əlavə etmək üçün əsas sistemi dəyişməyə ehtiyac yoxdur.
- **Versiya nəzarəti:** Hər plugin öz versiyası ilə saxlanılır və yeniləmə zamanı uyğunsuzluqların qarşısı alınır.
- **UI idarəsi:** Administratorlar pluginləri asanlıqla aktiv/deaktiv edə və konfiqurasiyanı dəyişə bilir.

## Gələcək inkişaf yolları və risklər
- Ödənişli və ya lisenziyalı pluginlərin idarə mexanizmi.
- Pluginlər arasında asılılıqların avtomatik yoxlanılması.
- Zərərli kod riski qarşısında əlavə təhlükəsizlik skanerlərinin tətbiqi.
