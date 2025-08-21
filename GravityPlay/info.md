INSTRUCTIONS RAPIDES (clavier)
- 1/2/3 : gravité Lune / Mars / Terre
- G / H : diminuer / augmenter g
- F1/F2/F3/F4/F5 : 1 / 30 / 60 / 120 / 850 FPS (best-effort render pacing)
- '-' / '+' : slow motion (TimeScale) 0.02 .. 1.0
- C / S / D : spawn Circle / Square / Diamond (aléatoires)
- T : activer/désactiver collisions entre formes (actuellement optimisées cercle-cercle)

SOURIS
- 1er clic : choisir la position de spawn
- 2e clic : choisir la direction (la vitesse initiale est appliquée dans ce sens)

NOTES
- Physique à pas fixe (240 Hz) avec rendu découplé (FPS cible variable) et TimeScale pour le slow motion.
- Collisions murs avec restitution & friction tangente ; frottement de roulement au sol qui couple translation/rotation.
- Les collisions inter-formes sont limitées aux cercles-cercle pour un MVP fluide CPU-only ; on pourra étendre à SAT pour polygones ensuite.
- Tout est paramétrable via la classe Config (comme demandé : réglages "dans le code").