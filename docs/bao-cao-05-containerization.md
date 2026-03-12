# Báo cáo: Đóng gói ứng dụng bằng container (05 - Containerization)

## 1. Giới thiệu

Trong bài lab này, mục tiêu là đóng gói (containerize) hai ứng dụng của Contoso gồm:

- Backend: ứng dụng Java (micro social media API)
- Frontend: ứng dụng .NET (UI)

Sau khi đóng gói thành các container image, hai ứng dụng sẽ được điều phối chạy cùng nhau thông qua Docker Compose để có thể triển khai nhất quán trên nhiều nền tảng.

Tài liệu tham khảo chính: [docs/05-containerization.vi.md](docs/05-containerization.vi.md) (dịch từ [docs/05-containerization.md](docs/05-containerization.md)).

## 2. Bối cảnh và mục tiêu

### 2.1. Bối cảnh

Contoso kinh doanh sản phẩm cho các hoạt động ngoài trời. Bộ phận marketing muốn triển khai một website mạng xã hội nhỏ để quảng bá sản phẩm. Hệ thống hiện có 2 phần tách rời:

- Backend Java: cung cấp API
- Frontend .NET: giao diện người dùng

### 2.2. Mục tiêu

- Tạo Dockerfile cho backend Java và frontend .NET
- Build container image cho từng ứng dụng
- Chạy thử từng container để kiểm tra hoạt động
- Tạo file compose.yaml để điều phối chạy đồng thời 2 container
- Xác minh hệ thống chạy được bằng cách truy cập web (http://localhost:3030)

## 3. Tổng quan kiến trúc và luồng chạy

### 3.1. Thành phần

- Backend service (Java)
  - Lắng nghe cổng 8080 trong container
  - Khi chạy độc lập: ánh xạ host port 8080 → container port 8080
- Frontend service (.NET)
  - Lắng nghe cổng 8080 trong container
  - Khi chạy độc lập: ánh xạ host port 3030 → container port 8080

### 3.2. Biến môi trường quan trọng

- Backend Java
  - CODESPACE_NAME
  - GITHUB_CODESPACES_PORT_FORWARDING_DOMAIN
  - Tạo file SQLite sns_api.db ngay trong image (không copy từ host)
- Frontend .NET
  - ApiSettings__BaseUrl: URL trỏ đến API backend (ví dụ: http://localhost:8080/api khi chạy độc lập)

## 4. Chuẩn bị môi trường

### 4.1. Điều kiện trước

Thực hiện các bước chuẩn bị theo [README](README.md) ở thư mục gốc (đường dẫn tương đối từ docs/ là [README.md](README.md)).

### 4.2. Kiểm tra GitHub Copilot Agent Mode

- Mở GitHub Copilot trong VS Code / Codespaces
- Đảm bảo đang bật Agent Mode
- Chọn model: GPT-4.1 hoặc Claude Sonnet 4
- Đảm bảo đã cấu hình MCP Servers theo tài liệu setup: [docs/00-setup.md](docs/00-setup.md)

### 4.3. Chuẩn bị custom instructions

Thiết lập biến môi trường REPOSITORY_ROOT và copy custom instructions cho bài containerization.

Bash/Zsh:

```bash
REPOSITORY_ROOT=$(git rev-parse --show-toplevel)
cp -r $REPOSITORY_ROOT/docs/custom-instructions/containerization/. \
      $REPOSITORY_ROOT/.github/
```

PowerShell:

```powershell
$REPOSITORY_ROOT = git rev-parse --show-toplevel
Copy-Item -Path $REPOSITORY_ROOT/docs/custom-instructions/containerization/* `
          -Destination $REPOSITORY_ROOT/.github/ -Recurse -Force
```

Mục đích: giúp Copilot bám sát yêu cầu bài lab, tuân thủ tiêu chuẩn repo và giảm sai lệch khi sinh Dockerfile/compose.

## 5. Containerize ứng dụng Java (backend)

### 5.1. Phạm vi

- Source của ứng dụng Java nằm tại java/socialapp
- Tạo Dockerfile tên Dockerfile.java ở thư mục gốc repo

### 5.2. Yêu cầu kỹ thuật

- Dùng Microsoft OpenJDK 21
- Multi-stage build
- Trích xuất JRE từ JDK (giảm dung lượng image runtime)
- Expose/target port: 8080
- Pass biến môi trường CODESPACE_NAME và GITHUB_CODESPACES_PORT_FORWARDING_DOMAIN từ host vào container
- Tạo file SQLite sns_api.db trong image (không copy từ máy host)

### 5.3. Prompt tham khảo (dùng với Copilot Agent Mode)

```text
Tôi muốn build một container image cho ứng dụng Java. Hãy làm theo các yêu cầu sau.

- Trước tiên, hãy liệt kê tất cả các bước bạn sẽ thực hiện.
- Ứng dụng Java nằm tại `java/socialapp`.
- Thư mục làm việc hiện tại là thư mục gốc của repository.
- Tạo một Dockerfile tên là `Dockerfile.java`.
- Dùng Microsoft OpenJDK 21.
- Dùng cách build nhiều giai đoạn (multi-stage build).
- Trích xuất JRE từ JDK.
- Dùng cổng (port) đích là `8080` cho container image.
- Thêm hai biến môi trường `CODESPACE_NAME` và `GITHUB_CODESPACES_PORT_FORWARDING_DOMAIN` từ host vào container image.
- Tạo một file cơ sở dữ liệu SQLite tên `sns_api.db` bên trong container image. KHÔNG copy file này từ host.
```

### 5.4. Build image và kiểm tra

Prompt build image:

```text
Hãy dùng `Dockerfile.java` để build một container image.

- Dùng `contoso-backend` làm tên container image.
- Dùng `latest` làm tag của container image.
- Xác minh container image được build đúng.
- Nếu build thất bại, hãy phân tích nguyên nhân và sửa.
```

Kết quả kỳ vọng:

- Image contoso-backend:latest build thành công
- Không còn lỗi Gradle/build
- Container có thể khởi động và lắng nghe cổng 8080

### 5.5. Chạy container và xác minh

Prompt chạy container:

```text
Hãy dùng container image vừa build để chạy một container và xác minh ứng dụng chạy bình thường.

- Dùng cổng (host port) là `8080`.
- Giá trị của `CODESPACE_NAME` và `GITHUB_CODESPACES_PORT_FORWARDING_DOMAIN` phải lấy từ GitHub Codespaces.
```

Cách kiểm tra (minh chứng):

- Kiểm tra container đang chạy (docker ps)
- Truy cập thử endpoint (tuỳ API), hoặc kiểm tra log container để thấy ứng dụng start thành công
- Ghi lại log/ảnh chụp màn hình kết quả

## 6. Containerize ứng dụng .NET (frontend)

### 6.1. Phạm vi

- Source của ứng dụng .NET nằm tại dotnet
- Tạo Dockerfile tên Dockerfile.dotnet ở thư mục gốc repo

### 6.2. Yêu cầu kỹ thuật

- Dùng .NET 9
- Multi-stage build
- Expose/target port: 8080
- Thiết lập biến môi trường ApiSettings__BaseUrl trỏ về backend Java, ví dụ: http://localhost:8080/api (khi chạy độc lập)

### 6.3. Prompt tham khảo

```text
Tôi muốn build một container image cho ứng dụng .NET. Hãy làm theo các yêu cầu sau.

- Trước tiên, hãy liệt kê tất cả các bước bạn sẽ thực hiện.
- Ứng dụng .NET nằm tại `dotnet`.
- Thư mục làm việc hiện tại là thư mục gốc của repository.
- Tạo một Dockerfile tên là `Dockerfile.dotnet`.
- Dùng .NET 9.
- Dùng cách build nhiều giai đoạn (multi-stage build).
- Dùng cổng (port) đích là `8080` cho container image.
- Thêm biến môi trường `ApiSettings__BaseUrl` vào container. Giá trị phải trỏ tới ứng dụng Java: `http://localhost:8080/api`.
```

### 6.4. Build image và kiểm tra

Prompt build image:

```text
Hãy dùng `Dockerfile.dotnet` để build một container image.

- Dùng `contoso-frontend` làm tên container image.
- Dùng `latest` làm tag của container image.
- Xác minh container image được build đúng.
- Nếu build thất bại, hãy phân tích nguyên nhân và sửa.
```

Kết quả kỳ vọng:

- Image contoso-frontend:latest build thành công
- Container có thể start và phục vụ web UI

### 6.5. Chạy container và xác minh

Prompt chạy container:

```text
Hãy dùng container image vừa build để chạy một container và xác minh ứng dụng chạy bình thường.

- Dùng cổng (host port) là `3030`.
- Truyền biến môi trường `ApiSettings__BaseUrl` giá trị `http://localhost:8080/api`.
```

Cách kiểm tra (minh chứng):

- Truy cập http://localhost:3030
- Kiểm tra UI hiển thị
- Lưu ý: ở bước chạy độc lập, frontend và backend có thể chưa “biết” nhau đầy đủ để giao tiếp đúng như khi chạy compose

### 6.6. Dọn dẹp tài nguyên (theo hướng dẫn)

Mục đích: đảm bảo trạng thái sạch trước khi sang bước Docker Compose.

Prompt tham khảo:

```text
Hãy gỡ (remove) cả hai container Java và .NET, cùng với các container image tương ứng.
```

## 7. Điều phối các container bằng Docker Compose

### 7.1. Mục tiêu

- Tạo compose.yaml để chạy cả backend và frontend cùng một network
- Thiết lập biến môi trường và mapping cổng đúng yêu cầu

### 7.2. Yêu cầu

- Backend:
  - Dockerfile.java
  - container name: contoso-backend
  - ports: 8080:8080
  - network: contoso
  - pass CODESPACE_NAME, GITHUB_CODESPACES_PORT_FORWARDING_DOMAIN
- Frontend:
  - Dockerfile.dotnet
  - container name: contoso-frontend
  - ports: 3030:8080
  - network: contoso
  - ApiSettings__BaseUrl trỏ đến /api của backend

### 7.3. Prompt tham khảo để tạo compose.yaml

```text
Tôi muốn tạo một Docker Compose file. Hãy làm theo các yêu cầu sau.

- Trước tiên, hãy liệt kê tất cả các bước bạn sẽ thực hiện.
- Thư mục làm việc hiện tại là thư mục gốc của repository.
- Dùng `Dockerfile.java` cho backend.
- Dùng `Dockerfile.dotnet` cho frontend.
- Tạo file Docker Compose tên `compose.yaml`.
- Dùng `contoso` làm tên network.
- Dùng `contoso-backend` làm container name cho ứng dụng Java. Target port là 8080, host port là 8080.
- Dùng `contoso-frontend` làm container name cho ứng dụng .NET. Target port là 8080, host port là 3030.
- Thêm hai biến môi trường `CODESPACE_NAME` và `GITHUB_CODESPACES_PORT_FORWARDING_DOMAIN` từ host vào container Java.
- Thêm biến môi trường `ApiSettings__BaseUrl` vào container .NET. Giá trị phải trỏ tới `/api` của ứng dụng Java.
```

### 7.4. Chạy compose và xác minh

Prompt chạy compose:

```text
Hãy chạy Docker Compose file và xác minh tất cả các ứng dụng đều chạy bình thường.
```

Cách kiểm tra (minh chứng):

- Kiểm tra cả 2 container đang chạy
- Truy cập http://localhost:3030
- Thực hiện một thao tác UI (nếu có) để xác minh frontend gọi được API
- Lưu log hoặc ảnh chụp màn hình

## 8. Kết quả đạt được

Sau khi hoàn thành, hệ thống đạt các kết quả kỳ vọng:

- Tạo được Dockerfile cho Java và .NET
- Build thành công 2 image: contoso-backend:latest và contoso-frontend:latest
- Chạy được 2 container độc lập để kiểm tra
- Tạo được compose.yaml để điều phối và chạy đồng thời
- Xác minh UI truy cập từ trình duyệt ở http://localhost:3030

## 9. Nhận xét và bài học rút ra

- Multi-stage build giúp giảm kích thước image runtime, tách bước build và chạy.
- Việc tách backend/frontend thành container độc lập giúp triển khai linh hoạt và nhất quán.
- Docker Compose giúp chuẩn hoá cấu hình chạy nhiều service: network, port mapping, env vars.
- Cần đặc biệt chú ý biến môi trường endpoint API và cấu hình networking khi chạy nhiều container.

## 10. Phụ lục (minh chứng)

Gợi ý nội dung bạn có thể bổ sung để báo cáo “đủ điểm”:

- Ảnh chụp màn hình:
  - Kết quả build image
  - Danh sách container đang chạy
  - Trang web chạy tại http://localhost:3030
- Trích log quan trọng khi backend/frontend khởi động
- Ghi chú lỗi gặp phải (nếu có) và cách khắc phục
