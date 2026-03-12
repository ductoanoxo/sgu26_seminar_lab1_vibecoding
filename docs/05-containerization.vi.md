# 05: Containerization (Đóng gói bằng container)

## Bối cảnh

Contoso là một công ty bán các sản phẩm phục vụ nhiều hoạt động ngoài trời. Bộ phận marketing của Contoso muốn ra mắt một website mạng xã hội nhỏ theo hướng microservice để quảng bá sản phẩm tới khách hàng hiện tại và khách hàng tiềm năng.

Hiện tại họ có cả ứng dụng backend viết bằng Java và ứng dụng frontend viết bằng .NET. Họ muốn đóng gói (containerize) cả hai ứng dụng để có thể triển khai lên bất kỳ nền tảng nào.

Bây giờ, với vai trò là một DevOps engineer, bạn cần đóng gói cả hai ứng dụng.

## Yêu cầu trước

Tham khảo tài liệu [README](../README.md) để chuẩn bị.

## Bắt đầu

- [Kiểm tra GitHub Copilot Agent Mode](#kiểm-tra-github-copilot-agent-mode)
- [Chuẩn bị Custom Instructions](#chuẩn-bị-custom-instructions)
- [Containerize ứng dụng Java](#containerize-ứng-dụng-java)
- [Containerize ứng dụng .NET](#containerize-ứng-dụng-net)
- [Điều phối các container](#điều-phối-các-container)

### Kiểm tra GitHub Copilot Agent Mode

1. Nhấp biểu tượng GitHub Copilot ở phía trên của GitHub Codespace hoặc VS Code để mở cửa sổ GitHub Copilot.

   ![Open GitHub Copilot Chat](./images/setup-02.png)

1. Nếu được yêu cầu đăng nhập hoặc đăng ký, hãy thực hiện. GitHub Copilot có thể dùng miễn phí (tuỳ chính sách/điều kiện của bạn).
1. Đảm bảo bạn đang dùng GitHub Copilot ở chế độ Agent Mode.

   ![GitHub Copilot Agent Mode](./images/setup-03.png)

1. Chọn model là `GPT-4.1` hoặc `Claude Sonnet 4`.
1. Đảm bảo bạn đã cấu hình [MCP Servers](./00-setup.md#set-up-mcp-servers).

### Chuẩn bị Custom Instructions

1. Thiết lập biến môi trường `$REPOSITORY_ROOT`.

   ```bash
   # bash/zsh
   REPOSITORY_ROOT=$(git rev-parse --show-toplevel)
   ```

   ```powershell
   # PowerShell
   $REPOSITORY_ROOT = git rev-parse --show-toplevel
   ```

1. Sao chép custom instructions.

    ```bash
    # bash/zsh
    cp -r $REPOSITORY_ROOT/docs/custom-instructions/containerization/. \
          $REPOSITORY_ROOT/.github/
    ```

    ```powershell
    # PowerShell
    Copy-Item -Path $REPOSITORY_ROOT/docs/custom-instructions/containerization/* `
              -Destination $REPOSITORY_ROOT/.github/ -Recurse -Force
    ```

### Containerize ứng dụng Java

1. Đảm bảo bạn đang dùng GitHub Copilot Agent Mode với model `Claude Sonnet 4` hoặc `GPT-4.1`.
1. Dùng prompt như bên dưới để build container image cho ứng dụng Java.

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

1. Nhấp nút ![the keep button image](https://img.shields.io/badge/keep-blue) trong GitHub Copilot để chấp nhận (take) các thay đổi.

1. Khi `Dockerfile.java` đã được tạo, build container image với prompt sau.

    ```text
    Hãy dùng `Dockerfile.java` để build một container image.

    - Dùng `contoso-backend` làm tên container image.
    - Dùng `latest` làm tag của container image.
    - Xác minh container image được build đúng.
    - Nếu build thất bại, hãy phân tích nguyên nhân và sửa.
    ```

1. Nhấp nút ![the keep button image](https://img.shields.io/badge/keep-blue) trong GitHub Copilot để chấp nhận (take) các thay đổi.

1. Khi build thành công, chạy container image bằng prompt sau.

    ```text
    Hãy dùng container image vừa build để chạy một container và xác minh ứng dụng chạy bình thường.
    
    - Dùng cổng (host port) là `8080`.
    - Giá trị của `CODESPACE_NAME` và `GITHUB_CODESPACES_PORT_FORWARDING_DOMAIN` phải lấy từ GitHub Codespaces.
    ```

### Containerize ứng dụng .NET

1. Đảm bảo bạn đang dùng GitHub Copilot Agent Mode với model `Claude Sonnet 4` hoặc `GPT-4.1`.
1. Dùng prompt như bên dưới để build container image cho ứng dụng .NET.

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

1. Nhấp nút ![the keep button image](https://img.shields.io/badge/keep-blue) trong GitHub Copilot để chấp nhận (take) các thay đổi.

1. Khi `Dockerfile.dotnet` đã được tạo, build container image với prompt sau.

    ```text
    Hãy dùng `Dockerfile.dotnet` để build một container image.

    - Dùng `contoso-frontend` làm tên container image.
    - Dùng `latest` làm tag của container image.
    - Xác minh container image được build đúng.
    - Nếu build thất bại, hãy phân tích nguyên nhân và sửa.
    ```

1. Nhấp nút ![the keep button image](https://img.shields.io/badge/keep-blue) trong GitHub Copilot để chấp nhận (take) các thay đổi.

1. Khi build thành công, chạy container image bằng prompt sau.

    ```text
    Hãy dùng container image vừa build để chạy một container và xác minh ứng dụng chạy bình thường.
    
    - Dùng cổng (host port) là `3030`.
    - Truyền biến môi trường `ApiSettings__BaseUrl` giá trị `http://localhost:8080/api`.
    ```

1. Đảm bảo cả frontend và backend hiện KHÔNG giao tiếp được với nhau vì chúng chưa “biết” nhau. Chạy prompt như bên dưới.

    ```text
    Hãy gỡ (remove) cả hai container Java và .NET, cùng với các container image tương ứng.
    ```

### Điều phối các container

1. Đảm bảo bạn đang dùng GitHub Copilot Agent Mode với model `Claude Sonnet 4` hoặc `GPT-4.1`.
1. Dùng prompt như bên dưới để tạo Docker Compose file.

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

1. Nhấp nút ![the keep button image](https://img.shields.io/badge/keep-blue) trong GitHub Copilot để chấp nhận (take) các thay đổi.

1. Khi file `compose.yaml` đã được tạo, chạy nó và xác minh cả hai ứng dụng chạy bình thường.

    ```text
    Hãy chạy Docker Compose file và xác minh tất cả các ứng dụng đều chạy bình thường.
    ```

1. Mở trình duyệt và truy cập `http://localhost:3030`, sau đó kiểm tra ứng dụng đã chạy ổn chưa.

---

Chúc mừng! 🎉 Bạn đã hoàn thành tất cả các buổi workshop thành công!
