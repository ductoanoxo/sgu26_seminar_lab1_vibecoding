# Stage 1: Build the application with Microsoft OpenJDK 21
FROM mcr.microsoft.com/openjdk/jdk:21-ubuntu AS builder

WORKDIR /app

# Copy Gradle wrapper and build files
COPY complete/java/socialapp/gradle gradle
COPY complete/java/socialapp/gradlew .
COPY complete/java/socialapp/gradlew.bat .
COPY complete/java/socialapp/build.gradle .
COPY complete/java/socialapp/settings.gradle .

# Download dependencies
RUN ./gradlew dependencies --no-daemon

# Copy source code
COPY complete/java/socialapp/src src

# Build the application
RUN ./gradlew bootJar --no-daemon

# Stage 2: Extract JRE from JDK using jlink
FROM mcr.microsoft.com/openjdk/jdk:21-ubuntu AS jre-builder

# Create a custom JRE with jlink
RUN jlink \
    --add-modules java.base,java.compiler,java.desktop,java.instrument,java.management,java.naming,java.net.http,java.prefs,java.rmi,java.scripting,java.security.jgss,java.security.sasl,java.sql,jdk.httpserver,jdk.management,jdk.unsupported \
    --strip-debug \
    --no-man-pages \
    --no-header-files \
    --compress=2 \
    --output /jre

# Stage 3: Runtime image with extracted JRE
FROM ubuntu:22.04

# Install SQLite for database creation
RUN apt-get update && \
    apt-get install -y sqlite3 && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Copy the custom JRE from jre-builder stage
COPY --from=jre-builder /jre /opt/jre

# Set up environment variables for Java
ENV JAVA_HOME=/opt/jre
ENV PATH="${JAVA_HOME}/bin:${PATH}"

WORKDIR /app

# Copy the built JAR from builder stage
COPY --from=builder /app/build/libs/*.jar app.jar

# Create SQLite database file
RUN sqlite3 /app/sns_api.db "VACUUM;"

# Add environment variables from host (these will be passed at runtime)
ARG CODESPACE_NAME
ARG GITHUB_CODESPACES_PORT_FORWARDING_DOMAIN
ENV CODESPACE_NAME=${CODESPACE_NAME}
ENV GITHUB_CODESPACES_PORT_FORWARDING_DOMAIN=${GITHUB_CODESPACES_PORT_FORWARDING_DOMAIN}

# Expose port 8080
EXPOSE 8080

# Run the application
ENTRYPOINT ["java", "-jar", "app.jar"]
