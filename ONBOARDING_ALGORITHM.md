# 📋 Algorithme du Parcours d'Onboarding - Documentation Technique

## 🎯 Vue d'ensemble

Le système d'onboarding utilise un **algorithme de scoring hybride** combinant :

- **Questions fermées** (préférences déclarées)
- **Interactions de swipe** (préférences révélées)
- **Scoring multi-critères** avec pondération

---

## 🔄 Flux du Parcours

```typescript
Intro → Questions (4) → Swipe (7 animes) → Résultats → Inscription
 0%      15%/30%/45%/60%    60%→100%        100%
```

### Étapes détaillées

1. **intro.vue** : Présentation et démarrage
2. **questions.vue** : 4 questions fermées (mood, universe, duration, language)
3. **swipe.vue** : Évaluation de 7 animes candidats
4. **result.vue** : Affichage de la meilleure recommandation + CTAs

---

## 🧮 Algorithme de Scoring

### Formule principale

```typescript
score = (1.0 × genreSum) + (0.7 × studioSum) + questionBoosts + popularityPenalty
```

### 1. Scores des Genres (basés sur swipes)

- **Like** : +10 points par genre de l'anime
- **Dislike** : -10 points par genre de l'anime
- **Skip** : 0 point

### 2. Scores des Studios (basés sur swipes)

- **Like** : +10 points par studio de l'anime
- **Dislike** : -10 points par studio de l'anime
- **Coefficient** : 0.7 (moins important que les genres)

### 3. Boosts des Questions (+6 à +8 points)

#### Mood → Genres

```typescript
const MOOD_TO_GENRES = {
  'action': ['Action', 'Aventure', 'Shonen'],
  'romance': ['Romance', 'Shojo', 'Slice of Life'],
  'mystery': ['Mystère', 'Thriller', 'Seinen'],
  'comedy': ['Comédie', 'Slice of Life', 'Shonen'],
  'drama': ['Drame', 'Seinen', 'Josei'],
  'fantasy': ['Fantasy', 'Aventure', 'Isekai']
}
```

#### Universe → Genres

```typescript
const UNIVERSE_TO_GENRES = {
  'modern': ['Slice of Life', 'Romance', 'Drame'],
  'historical': ['Historique', 'Drame', 'Seinen'],
  'futuristic': ['Sci-Fi', 'Mecha', 'Cyberpunk'],
  'fantasy': ['Fantasy', 'Magie', 'Isekai'],
  'school': ['École', 'Romance', 'Comédie'],
  'supernatural': ['Surnaturel', 'Horreur', 'Fantasy']
}
```

#### Durée

```typescript
const DURATION_RANGES = {
  'short': { min: 0, max: 12 },    // +8 si exact, +4 si proche
  'medium': { min: 13, max: 24 },  // +8 si exact, +4 si proche  
  'long': { min: 25, max: 999 }    // +8 si exact, +4 si proche
}
```

#### Langue

- **+6 points** si l'anime a la version demandée (VF/VostFR)

### 4. Pénalité de Popularité

```typescript
popularityPenalty = -(anime.popularity / 500)
```

- *Favorise les animes moins mainstream*

---

## 📊 Sources de Données

### Backend Endpoints

Les seules requêtes envoyées au backend sont :

1. **`/api/Anime/recommendation-dataset`** : 50 animes candidats pour la recommandation finale  
2. **`/api/Anime/onboarding-candidates`** : 7 animes utilisés dans le swipe

### Persistance (SessionStorage)

```typescript
interface OnboardingState {
  answers: OnboardingAnswers        // Réponses aux 4 questions
  swipeInteractions: SwipeInteraction[]  // Historique des swipes
  recommendedAnimes: RecommendationResult[]  // Top 5 final
}
```

Toutes les réponses aux questions et les interactions de swipe sont stockées uniquement côté client (via Pinia / SessionStorage) jusqu’à l’inscription éventuelle.

---

## 🎯 Normalisation et Résultats

### Score de Compatibilité Final

```typescript
compatibilityScore = Math.max(0, Math.min(100, score + 50))
```

- *Normalisation entre 0-100% avec offset de +50*

### Format de Sortie

```typescript
interface RecommendationResult {
  anime: Anime
  compatibilityScore: number  // 0-100%
  reasonTags: string[]       // ['genre:Action', 'Mood: action', 'duration:short']
}
```

---

## 🔄 Transfert vers le Compte Utilisateur

### Génération des Préférences

```typescript
// Genres (combinaison swipes + questions)
generateGenrePreferences(): Array<{genreName: string, score: number}>

// Studios (basé uniquement sur les swipes)  
generateStudioPreferences(): Array<{studioName: string, score: number}>
```

### Intégration API

- Les préférences sont envoyées lors de l'inscription  
- Ce transfert n’a lieu qu’au moment où l’utilisateur choisit de créer un compte. Aucune donnée n’est persistée avant.  
- Format compatible avec les endpoints backend existants  
- Permet la personnalisation immédiate des recommandations

---

## ⚡ Avantages de l'Approche

1. **Hybride** : Combine préférences déclarées et révélées
2. **Évolutif** : L'algorithme s'améliore avec plus d'interactions
3. **Personnalisé** : Chaque utilisateur a un profil unique
4. **Rapide** : Calcul côté client, pas de latence réseau
5. **Transparent** : `reasonTags` expliquent les recommandations

---

## 🏗️ Architecture Technique

### Composants Principaux

- **`useOnboardingScore.ts`** : Logique de scoring et recommandations
- **`onBoarding.ts` (store)** : État global et persistance
- **`SimpleProgressBar.vue`** : Barre de progression réutilisable
- **`OnbButton.vue`** : Boutons stylisés pour l'onboarding

### Pages du Parcours

- **`intro.vue`** : Page d'accueil avec présentation
- **`questions.vue`** : 4 questions fermées avec boutons
- **`swipe.vue`** : Interface de swipe avec 7 animes
- **`result.vue`** : Affichage recommandation + CTAs inscription
