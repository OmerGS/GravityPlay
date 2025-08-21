INSTRUCTIONS RAPIDES (clavier)
- 1/2/3 : gravit� Lune / Mars / Terre
- G / H : diminuer / augmenter g
- F1/F2/F3/F4/F5 : 1 / 30 / 60 / 120 / 850 FPS (best-effort render pacing)
- '-' / '+' : slow motion (TimeScale) 0.02 .. 1.0
- C / S / D : spawn Circle / Square / Diamond (al�atoires)
- T : activer/d�sactiver collisions entre formes (actuellement optimis�es cercle-cercle)

SOURIS
- 1er clic : choisir la position de spawn
- 2e clic : choisir la direction (la vitesse initiale est appliqu�e dans ce sens)

NOTES
- Physique � pas fixe (240 Hz) avec rendu d�coupl� (FPS cible variable) et TimeScale pour le slow motion.
- Collisions murs avec restitution & friction tangente ; frottement de roulement au sol qui couple translation/rotation.
- Les collisions inter-formes sont limit�es aux cercles-cercle pour un MVP fluide CPU-only ; on pourra �tendre � SAT pour polygones ensuite.
- Tout est param�trable via la classe Config (comme demand� : r�glages "dans le code").