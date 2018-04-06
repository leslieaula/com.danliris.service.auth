docker build -f Dockerfile.test.build -t com-danliris-service-auth-webapi:test-build .
docker create --name com-danliris-service-auth-webapi-test-build com-danliris-service-auth-webapi:test-build
mkdir bin
docker cp com-danliris-service-auth-webapi-test-build:/out/. ./bin/publish
docker build -f Dockerfile.test -t com-danliris-service-auth-webapi:test .
