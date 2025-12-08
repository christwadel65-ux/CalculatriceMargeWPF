from PIL import Image, ImageDraw
import os

# Créer une image pour l'icône (calculatrice simple)
size = 256
img = Image.new('RGBA', (size, size), color=(255, 255, 255, 0))
draw = ImageDraw.Draw(img)

# Fond bleu
draw.rectangle([10, 10, size-10, size-10], fill=(45, 156,
               219, 255), outline=(29, 53, 87, 255), width=3)

# Écran
draw.rectangle([30, 30, size-30, 100], fill=(255, 255, 255,
               255), outline=(29, 53, 87, 255), width=2)

# Texte "0"
draw.text((size//2-15, 50), "0", fill=(0, 0, 0, 255), font=None)

# Boutons numériques (grille 4x3)
button_width = (size - 60) // 3
button_height = (size - 120) // 4

for row in range(4):
    for col in range(3):
        x1 = 30 + col * (button_width + 8)
        y1 = 120 + row * (button_height + 8)
        x2 = x1 + button_width
        y2 = y1 + button_height
        draw.rectangle([x1, y1, x2, y2], fill=(
            255, 255, 255, 255), outline=(45, 156, 219, 255), width=2)

# Sauvegarder en PNG
png_path = "Images/app_icon.png"
img.save(png_path)
print(f"✓ Image créée: {png_path}")

# Convertir en ICO
try:
    icon_path = "Images/app_icon.ico"
    img_ico = img.resize((256, 256), Image.Resampling.LANCZOS)
    img_ico.save(icon_path, format='ICO')
    print(f"✓ Icône créée: {icon_path}")
except Exception as e:
    print(f"Erreur lors de la création de l'ICO: {e}")
    # Créer une version ICO avec tailles multiples
    sizes = [(16, 16), (32, 32), (64, 64), (128, 128), (256, 256)]
    icon_images = [img.resize(size, Image.Resampling.LANCZOS)
                   for size in sizes]
    icon_images[0].save(icon_path, sizes=[(s, s)
                        for s in [16, 32, 64, 128, 256]])
    print(f"✓ Icône multi-résolution créée: {icon_path}")
