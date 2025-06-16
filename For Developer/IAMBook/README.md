# IAMBook

Bu sənəd WebAdminPanel modulunda istifadəçi, rol və icazə idarəçiliyinin quruluşunu izah edir.

## Əsas anlayışlar
- **İstifadəçi idarəçiliyi** – sistemdə yeni istifadəçi yaratmaq, mövcud istifadəçini redaktə etmək və ya silmək imkanı.
- **Rol və icazələr** – hər rol üçün icazələrin matrisi, qruplar və alt qruplar üzrə iyerarxik bölünmə.
- **JIT səlahiyyət ötürülməsi** – məhdud müddətlik və ya şərtli icazələr vermək.
- **Modul və sahə üzrə funksionallıq açarları** – hər modul və ya spesifik sahə üçün funksiyanın aktiv/deaktiv edilməsi.
- **Rol simulyasiyası** – menyu və imkanların seçilmiş rol kimi necə göründüyünü sınaqdan keçirmək.

## İstifadə qaydası
1. `Users` səhifəsində istifadəçilərin siyahısı göstərilir. **Yeni** düyməsi ilə əlavə edə, mövcud qeydlər üzərində **Düzəliş** və **Sil** əməliyyatlarını icra etmək olar.
2. `Roles` səhifəsində rollar və onların icazələri idarə olunur. Rol əlavə etdikdə və ya redaktə etdikdə icazələri seçmək mümkündür.
3. `Permission Matrix` səhifəsi iyerarxik qruplarla icazələrin tam xəritəsini təqdim edir. Burada JIT səlahiyyət ötürülməsi üçün müddət təyin etmək və modul/sahə üzrə funksionallığı aktivləşdirmək olar.
4. Naviqasiya menyusundakı rollar siyahısından seçərək rol simulyasiyasını aktivləşdirmək mümkündür.

## Üstünlüklər
- Detallı icazə nəzarəti və qruplaşdırma böyük müəssisələrdə çevik idarəetmə təmin edir.
- JIT səlahiyyətləri sayəsində təhlükəsizlik artır, hüquqlar yalnız lazım olduqda verilir.
- Rol simulyasiyası UI dəyişikliklərini dərhal yoxlamağa kömək edir.

