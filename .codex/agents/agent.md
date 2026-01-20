# Guide de collaboration multi-agent

## Objectif
Travailler avec un ensemble d'agents specialises, connectes a Notion, afin de reduire la fenetre de contexte tout en conservant un contexte persistant.

## Fonctionnement general
- Chaque metier correspond a un fichier agent dedie dans `.codex/agents/`.
- L'agent a utiliser doit etre explicitement mentionne dans le prompt.
- Les sources de verite et responsabilites sont propres a chaque agent.
- Les agents peuvent s'appuyer sur Notion pour maintenir le contexte persistant.
- Charger ce guide global a chaque session pour cadrer le contexte multi-agent.

## Agents disponibles
- `product` -> `.codex/agents/product.md`
- `dev-fullstack` -> `.codex/agents/dev-fullstack.md`
- `data-scientist` -> `.codex/agents/data-scientist.md`
- `qa-analyst` -> `.codex/agents/qa-analyst.md`
- `devops-engineer` -> `.codex/agents/devops-engineer.md`

## Comment appeler un agent
Exemples :
- "Charge l'agent `product` depuis `.codex/agents/product.md` et reponds en tant que lui."
- "Utilise l'agent `product` (fichier `.codex/agents/product.md`) pour cadrer la prochaine release."
- "Charge l'agent `dev-fullstack` depuis `.codex/agents/dev-fullstack.md` et reponds en tant que lui."
- "Charge l'agent `data-scientist` depuis `.codex/agents/data-scientist.md` et reponds en tant que lui."
- "Charge l'agent `qa-analyst` depuis `.codex/agents/qa-analyst.md` et reponds en tant que lui."
- "Charge l'agent `devops-engineer` depuis `.codex/agents/devops-engineer.md` et reponds en tant que lui."

## Regles de collaboration
- Prioriser la clarification d'intention avant l'execution.
- Limiter le contexte a l'essentiel par agent.
- Referencer les pages Notion pertinentes quand c'est utile.
- Rester concis et actionnable dans les livrables.
- Workflow par defaut: ideation -> creation d'une release avec intentions -> brainstorming et cadrage -> validation du perimetre -> creation du tag release (ex: 0.4.0) -> user stories en base "Delivery - sprint actif".
- Implémentation: `dev-fullstack` couvre les tags "Front-End" et "Back-End"; `data-scientist` couvre les tags "Agent IA" et "Machine Learning".
- Coordination: si une fonctionnalite touche a la data et au fullstack, `data-scientist` et `dev-fullstack` doivent se synchroniser.
- Regle de commit: `[agent] type(scope): message court` (types: feat, fix, refactor, test, docs, chore).

## Notion
- MCP Notion active pour lire/ecrire.
- Les agents doivent noter les decisions importantes dans Notion.
