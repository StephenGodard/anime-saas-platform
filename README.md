# Animesphere

Bienvenue dans le projet **Animesphere** 🎯🚀  
Ce projet est une plateforme SaaS complète permettant de recommander, gérer et suivre facilement les anime de saison. avec un UX agréable et un design moderne.

---

## 📚 Présentation globale

Le projet est découpé en **plusieurs microservices** :

| Service | Description |
|:---|:---|
| **anime-saas-api** | API backend développée en **.NET 8** pour la gestion des données utilisateur, des animes, de la watchlist et des préférences. |
| **anime-saas-front** | Frontend développé avec **Nuxt 3** pour la partie front-end de l'application. |
| **anime-saas-mlservice** | Service de **Machine Learning** générant les recommandations personnalisées en fonction des préférences des utilisateurs. |
| **anime-saas-agent** | Microservice Python chargé de collecter et enrichir automatiquement les données d'animes (depuis AniList)|
| **MySQL** | Base de données relationnelle stockant les utilisateurs, animes, préférences, interactions et watchlists. |

L'infrastructure est entièrement **dockerisée** pour simplifier le développement, les tests et le déploiement 🚀.

---

## 🗺️  Schéma d'architecture

Voici l'architecture complète du projet
![Schéma d'architecture](assets/img/Architecture.png)

## 📖 Documentation du projet

La documentation complète est disponible ici :  
👉 [Consulter sur Notion](https://animesphere.notion.site/)

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
make up
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
| `/anime-saas-api/` | Backend API |
| `/anime-saas-front/` | Frontend Nuxt 3 |
| `/anime-saas-mlservice/` | Service ML Python FastApi |
| `/anime-saas-agent/` | Agent Scrapping Python |
| `/anime-saas-landing/` | Landing Page |
| `/anime-saas-reverseproxy/` | Reverse Proxy Nginx |
| `docker-compose.dev.yml` | Orchestration Docker pour l'environnement **Développement** |
| `docker-compose.prod.yml` | Orchestration Docker pour l'environnement **Production** |
| `Makefile` | Automatisation des commandes courantes |
| `README.md` | Présentation du projet |

---

## 📦 Stack technique utilisée

- Frontend ➔ **Nuxt 3** (Vite)
- Backend ➔ **.NET 8 API Web**
- Machine Learning ➔ **Python + FastAPI**
- Agent IA ➔ **Python** (microservice autonome de scraping et enrichissement de données)
- Reverse Proxy ➔ **Nginx**
- Base de données ➔ **MySQL 8**
- Infrastructure ➔ **Docker + Docker Compose**

---

## 🧠 Commandes utiles

| Commande | Description |
|:---|:---|
| `make up` | Démarre tous les services en mode développement |
| `make prod` | Démarre tous les services en mode production |
| `make down` | Stoppe tous les services |
| `make clean` | Nettoie tous les containers, images et volumes |
| `make ps` | Affiche les containers en cours d'exécution |
| `make test` | Lance les tests |

---
