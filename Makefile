dev:
	docker-compose -f docker-compose.dev.yml up --build

prod:
	docker-compose -f docker-compose.prod.yml up --build

down:
	docker-compose down

clean:
	docker-compose down -v --remove-orphans
	docker system prune -af --volumes

ps:
	docker-compose ps