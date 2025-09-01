up:
	docker compose -f docker-compose.yml up --build

test:
	docker build -f Dockerfile.test -t anime-saas-api-tests .
	docker run --rm anime-saas-api-tests

down:
	docker compose down

clean:
	docker compose down -v --remove-orphans
	docker system prune -af --volumes

ps:
	docker compose ps

update-submodules:
	git submodule update --remote --recursive

scrape:
	docker compose run --rm anime-saas-agent python -m src.agent.core.scheduler --now --limit 10

scrape-random:
	docker compose run --rm anime-saas-agent python -m src.agent.core.scheduler --now --limit 10

scrape-popular:
	docker compose run --rm anime-saas-agent python -m src.agent.core.scheduler --now --limit 10 --no-random

scrape-no-ai:
	docker compose run --rm anime-saas-agent python -m src.agent.core.scheduler --now --limit 5 --no-random --no-complete-fields

scrape-test:
	docker compose run --rm anime-saas-agent python -m src.agent.core.scheduler --now --limit 1

backfill-images:
	docker compose run --rm anime-saas-agent python -m src.agent.core.scheduler --now --backfill --batch-size 200

backfill-images-test:
	docker compose run --rm anime-saas-agent python -m src.agent.core.scheduler --now --backfill --batch-size 10 --dry-run

backfill-images-small:
	docker compose run --rm anime-saas-agent python -m src.agent.core.scheduler --now --backfill --batch-size 50

rebuild-agent:
	docker compose build anime-saas-agent