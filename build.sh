docker build --no-cache -t messagesenderserviceapi:v0.001 -f src/MessageSenderServiceApi/Dockerfile .;
docker-compose stop;
docker network rm test-task-net;
docker network create test-task-net;
docker-compose up --build -d;