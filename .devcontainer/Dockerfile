FROM mcr.microsoft.com/devcontainers/dotnet:1-9.0-bookworm

# [Optional] Uncomment this section to install additional OS packages.
# RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
#     && apt-get -y install --no-install-recommends <your-package-list-here>

USER vscode

# Install global node packages.
RUN . /usr/local/share/nvm/nvm.sh \
    && nvm install --lts \
    && nvm use --lts \
    && npm install -g typescript

# Install NUKE build tool globally for the "vscode" user.
RUN dotnet tool install Nuke.GlobalTool --global

# Switch to root to install wasm-tools workload required for Blazor projects
USER root
RUN dotnet workload install wasm-tools

# Switch back to vscode user
USER vscode

# Make sure the PATH environment variable includes the .dotnet tools directory.
ENV PATH="${PATH}:/home/vscode/.dotnet/tools"
