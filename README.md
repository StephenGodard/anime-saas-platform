# Anime SaaS Platform

Bienvenue dans le projet **Anime SaaS Platform** 🎯🚀  
Ce projet est une plateforme SaaS complète permettant de recommander, gérer et suivre les anime de saison.

---

## 📚 Présentation globale

Le projet est découpé en **plusieurs microservices** :

| Service | Description |
|:---|:---|
| **anime-saas-api** | API backend développée en **.NET 9** pour la gestion des données utilisateur, des animes et de la watchlist. |
| **anime-saas-front** | Frontend développé avec **Nuxt 3** pour l'affichage des recommandations, de la watchlist et des calendrier de sortie |
| **anime-saas-mlservice** | Service de **Machine Learning Python** (FastAPI) générant les recommandations personnalisées. |
| **MySQL** | Base de données pour stocker les utilisateurs, watchlists, animes, etc.

L'infrastructure est entièrement **dockerisée** pour simplifier le développement, les tests et le déploiement 🚀.

---

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
| `docker-compose.dev.yml` | Orchestration Docker pour l'environnement **Développement** |
| `docker-compose.prod.yml` | Orchestration Docker pour l'environnement **Production** |
| `Makefile` | Automatisation des commandes courantes |
| `README.md` | Présentation du projet |

---

## 📦 Stack technique utilisée

- Frontend ➔ **Nuxt 3** (Node.js)
- Backend ➔ **.NET 9 API Web**
- Machine Learning ➔ **Python + FastAPI**
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

## 🎯 Objectif du projet

Construire une plateforme SaaS **moderne, évolutive et performante**, en utilisant des bonnes pratiques de microservices, CI/CD, et design scalable.

---

## 🧹 TODO

- Mise en place de CI/CD GitHub Actions
- Sécurisation des communications interservices
- Optimisation de la base de données
- Ajout de monitoring (Grafana / Prometheus)
- Déploiement cloud (Azure ou AWS)

---

**Let's build something amazing! 🚀🎯**