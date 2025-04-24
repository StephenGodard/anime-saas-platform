# Anime SaaS Platform

Bienvenue dans le projet **Anime SaaS Platform** 🎯🚀  
Ce projet est une plateforme SaaS complète permettant de recommander, gérer et suivre les anime de saison.

---

## 📚 Présentation globale

Le projet est découpé en **plusieurs microservices** :

| Service | Description |
|:---|:---|
| **anime-saas-api** | API backend développée en **.NET 9** pour la gestion des données utilisateur, des animes, de la watchlist et des préférences. |
| **anime-saas-front** | Frontend développé avec **Nuxt 3** pour l'affichage des recommandations, de la watchlist et du calendrier de sortie des animes. |
| **anime-saas-mlservice** | Service de **Machine Learning Python** (FastAPI) générant les recommandations personnalisées. |
| **anime-saas-agent** | Microservice Python autonome chargé de collecter et enrichir automatiquement les données d'animes (depuis MyAnimeList et d'autres sources). |
| **MySQL** | Base de données relationnelle stockant les utilisateurs, animes, préférences, interactions et watchlists. |

L'infrastructure est entièrement **dockerisée** pour simplifier le développement, les tests et le déploiement 🚀.

---

## 🗺️  Schéma d'architecture

Voici l'architecture complète du projet
![Schéma d'architecture](assets/img/Architecture.png)

## 📖 Documentation du projet

La documentation complète est disponible ici :  
👉 [Consulter sur Notion](https://tattered-letter-62f.notion.site/Anime-SaaS-Platforme-1bb32705124c8056bc3ff23392ebcf20)

## 🛠️ Démarrage rapide

1. Clonez ce dépôt avec les submodules :
```bash
git clone --recurse-submodules <url-du-repo>
```

2. Placez-vous dans le projet :
```bash
cd anime-saas-platform
```

3. Lancez l'environnement de développement :
```bash
make dev
```
Cela démarrera tous les services nécessaires en mode développement avec **hot reload**.

4. Pour arrêter les services :
```bash
make down
```

---

## ⚙️ Contenu du projet

| Dossier/Fichier | Rôle |
|:---|:---|
| `/anime-saas-api/` | Backend API (submodule Git) |
| `/anime-saas-front/` | Frontend Nuxt 3 (submodule Git) |
| `/anime-saas-mlservice/` | Service ML Python (submodule Git) |
| `/anime-saas-agent/` | Agent IA Python (submodule Git) |
| `docker-compose.dev.yml` | Orchestration Docker pour l'environnement **Développement** |
| `docker-compose.prod.yml` | Orchestration Docker pour l'environnement **Production** |
| `Makefile` | Automatisation des commandes courantes |
| `README.md` | Présentation du projet |

---

## 📦 Stack technique utilisée

- Frontend ➔ **Nuxt 3** (Node.js)
- Backend ➔ **.NET 9 API Web**
- Machine Learning ➔ **Python + FastAPI**
- Agent IA ➔ **Python** (microservice autonome de scraping et enrichissement de données)
- Base de données ➔ **MySQL 8**
- Infrastructure ➔ **Docker + Docker Compose**

---

## 🧠 Commandes utiles

| Commande | Description |
|:---|:---|
| `make dev` | Démarre tous les services en mode développement |
| `make prod` | Démarre tous les services en mode production |
| `make down` | Stoppe tous les services |
| `make clean` | Nettoie tous les containers, images et volumes |
| `make update-submodules` | Met à jour les submodules |

---

## ⚡ Important - Gestion des submodules Git

Quand vous clonez le projet, utilisez bien l'option :
```bash
git clone --recurse-submodules <url-du-repo>
```

Si vous oubliez ➔ pensez à initialiser et récupérer les submodules manuellement :
```bash
git submodule update --init --recursive
```

Pour mettre à jour les submodules :
```bash
git submodule update --remote
```

---

## 🧹 TODO

- ✅ Finaliser le back-end en fonction des contraintes UX (modèles, contrôleurs, services)
- 🧪 Ajouter des tests d’intégration (EF Core InMemory)
- 🎨 Finaliser les spécifications UX et démarrer les maquettes Figma
- 🌐 Implémenter le front Nuxt (onboarding, swipe, recommandations)
- 🧠 Développement du microservice ML avec Scikit-learn
- 📡 Intégration agent IA pour enrichir les données (MyAnimeList, web scraping)
- 🔐 Sécuriser l'API avec JWT + OAuth2
- 🚢 Déployer les services sur VPS + configurer les noms de domaines
- 📈 Mettre en place la télémétrie (logs, métriques, alertes)
- 🧼 Préparer la CI/CD GitHub Actions pour build + test + déploiement auto

---

**Let's build something amazing! 🚀🎯**