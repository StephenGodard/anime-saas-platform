dev:
	docker-compose -f docker-compose.dev.yml up --build

prod:
	docker-compose -f docker-compose.prod.yml up --build
test:
	docker build -f Dockerfile.test -t anime-saas-api-tests .
	docker run --rm anime-saas-api-tests

down:
	docker-compose down

clean:
	docker-compose down -v --remove-orphans
	docker system prune -af --volumes

ps:
	docker-compose ps

update-submodules:
	git submodule update --remote --recursive