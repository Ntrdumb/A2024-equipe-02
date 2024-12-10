# Stellar Sketch - Jeu de plateforme 2D

## Table des Matières
1. [Introduction](#introduction)
2. [Fonctionnalités Principales](#fonctionnalités-principales)
4. [Physiques et Collisions](#physiques-et-collisions)
5. [Design et Assets](#design-et-assets)
6. [Options du Jeu](#options-du-jeu)
7. [Tester le Jeu](#tester-le-jeu)
   - [Accès au contenu via GitHub](#accès-au-contenu-via-github)
   - [Tester avec la Build Windows](#tester-avec-la-build-windows)
8. [Pratiques d'Ingénierie Logicielle](#pratiques-dingénierie-logicielle)

---

## Introduction
*Stellar Sketch* est un jeu de plateforme 2D post-apocalyptique où les joueurs doivent résoudre des puzzles en utilisant un outil unique de dessin. Naviguez dans une station spatiale en ruine, évitez les dangers et gérez vos ressources pour atteindre votre objectif final.

---

## Fonctionnalités Principales
- **Dessin interactif** : Créez et effacez des plateformes pour progresser dans les niveaux.
- **Gestion des ressources** : Limite d’encre pour ajouter un défi stratégique.
- **Progression en zones** :
  - Zone 1 : Introduction aux mécaniques.
  - Zone 2 : Mise en pratique.
  - Zone 3 : Défi stratégique.
  - Zone 4 : Section finale avec piège.
- **Options personnalisables** : Réglages pour l’audio, l’affichage et les contrôles.

---

## Physiques et Collisions
- Utilisation de **Rigidbody 2D** pour les interactions physiques.
- Optimisation avec des **Tilemap Collider 2D** pour minimiser les calculs inutiles.
- Système de collision basé sur des composants précis pour les ennemis et les objets interactifs.

---

## Design et Assets
- **Unity Tilemap** : Utilisé pour concevoir rapidement les niveaux avec une grille modulaire.
- **Effets sonores** : Intégrés pour signaler les actions importantes, comme la recharge d’encre.

---

## Options du Jeu
- **Audio** : Réglage du volume global, de la musique et des effets sonores.  
- **Affichage** : Options de résolution et mode plein écran.  
- **Contrôles** : Possibilité de personnaliser les touches assignées.  
- Les paramètres sont persistants grâce à **PlayerPrefs**, garantissant leur sauvegarde entre les sessions.

---

## Tester le Jeu

### Étapes pour cloner le projet et le lancer dans Unity

1. **Cloner le dépôt GitHub**  
   - Accédez à la page GitHub du projet.  
   - Copiez l'URL du dépôt en cliquant sur le bouton **Code** puis sur **Copy**.  
   - Ouvrez un terminal ou un outil Git (comme Git Bash) et exécutez la commande suivante :  
     ```bash
     git clone <URL_DU_DEPOT>
     ```
   - Remplacez `<URL_DU_DEPOT>` par l’URL copiée.  
   - Une fois terminé, un dossier contenant tous les fichiers du projet sera créé.

2. **Ouvrir le projet dans Unity**  
   - Lancez Unity Hub.  
   - Cliquez sur le bouton **Add** (Ajouter).  
   - Sélectionnez le dossier cloné contenant le projet.  
   - Une fois ajouté, ouvrez le projet en cliquant dessus dans Unity Hub.

3. **Lancer le jeu dans Unity**  
   - Une fois le projet ouvert dans Unity, cliquez sur le bouton **Play** situé en haut de l’éditeur Unity.  
   - Vous pourrez interagir directement avec le jeu dans l'éditeur.  
   - Explorez les fonctionnalités et effectuez vos tests.

### Étapes pour utiliser la build sans Unity

1. **Télécharger la build Windows**  
   - Accédez à la section **Releases** sur la page GitHub du projet (si disponible).  
   - Téléchargez la version la plus récente de la build Windows.  
   - Le fichier téléchargé sera généralement un dossier compressé (.zip).

2. **Extraire la build**  
   - Décompressez le dossier téléchargé à l’aide d’un outil comme WinRAR ou 7-Zip.  
   - Une fois extrait, accédez au dossier contenant les fichiers de la build.

3. **Lancer le jeu**  
   - Localisez le fichier exécutable du jeu (fichier `.exe`).  
   - Double-cliquez sur ce fichier pour lancer le jeu.  
   - Vous pourrez ainsi tester toutes les fonctionnalités directement, sans besoin d’installer Unity.

---

# Pratiques d'Ingénierie Logicielle
   - Contrôle de version : Utilisation de GitHub pour gérer les branches, les commits et les issues.
   - Stratégie de déploiement : Build Windows générée directement depuis Unity pour des tests rapides et une distribution simplifiée.