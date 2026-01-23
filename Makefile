up:
	docker compose -f docker-compose.yml up --build

test:
	docker build -f anime-saas-api/Dockerfile.test -t anime-saas-api-tests anime-saas-api
	docker run --rm anime-saas-api-tests

prod:
	docker compose -f docker-compose.prod.yml up --build
down:
	docker compose down

clean:
	docker compose down -v --remove-orphans
	docker system prune -af --volumes

ps:
	docker compose ps
