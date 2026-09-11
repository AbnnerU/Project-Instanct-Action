# KitWright MCP - instalacao local

Data: 2026-09-11

## Pacote e compatibilidade

- Unity: 6000.5.4f1.
- KitWright: 1.0.0.
- Origem: https://github.com/kitwright/unity-mcp.git
- Commit importado: ce38b6da0f80b5ff23444c3ae2308123ff5f24a6.
- Instalacao inicial pelo Git no Unity Package Manager.
- Instalacao final incorporada em `Packages/com.kitwright.unity.mcp`, com referencia `file:com.kitwright.unity.mcp` no manifest.

O pacote original falhou com CS0619 em `Editor/Tools/Helpers/ObjectIdCodec.cs`: `SerializedProperty.objectReferenceInstanceIDValue` ja e proibido nesta versao do Unity. A condicao para usar `objectReferenceEntityIdValue.IsValid()` foi alterada de `UNITY_6000_6_OR_NEWER` para `UNITY_6000_5_OR_NEWER`. A copia incorporada preserva essa correcao fora do cache. Ao atualizar o pacote, conferir se a correcao ja existe na versao nova antes de substituir esta copia.

## Conexao

- Transporte: HTTP MCP local, Broker Mode ativo.
- Endereco: http://127.0.0.1:8885/
- URL configurada no Codex: http://127.0.0.1:8885/p/f3417200/
- O sufixo identifica este projeto e evita conectar a outro editor por engano.
- Perfil: core (54 ferramentas expostas nesta revisao).
- Inicializacao automatica: habilitada.
- Configuracao e skill geradas pelo botao Codex > Configure + Skills.
- Reiniciar o Codex para carregar o novo servidor na lista nativa de ferramentas. A comunicacao HTTP MCP foi validada a partir desta tarefa, mas o registro nativo desta sessao nao foi recarregado.

## Verificacoes realizadas

- Handshake initialize retornou o projeto Project Instanct Action e versao 1.0.0.
- Leitura de unity://project/context.
- Leitura da SampleScene e sua hierarquia.
- Criacao de MCP_Test_Object na SampleScene, confirmacao por nova consulta e remocao.
- Entrada no Play Mode, confirmacao de isPlaying=true e saida.
- Comunicacao mantida pelo Broker durante o ciclo de Play Mode.
- Package Manager confirmou KitWright 1.0.0 como pacote Custom/Embedded.
- Verificacao final: Edit Mode, SampleScene sem alteracoes nao salvas, oito objetos raiz, objeto temporario ausente e nenhum erro no Console.
- Nenhuma cena, prefab ou dado do jogo foi salvo pelos testes. A cena originalmente limpa foi reaberta do disco ao final para remover a marcacao de alteracao dos testes.

Durante a configuracao houve um timeout de chamada porque um menu do Unity estava aberto, e uma tentativa de snippet teve erro de compilacao. O menu foi fechado e a operacao repetida com sucesso. Esses erros de configuracao nao permaneceram na verificacao final. Avisos preexistentes dos scripts do jogo nao foram corrigidos por esta instalacao.

## Arquivos criados ou alterados

- `Packages/manifest.json`.
- `Packages/packages-lock.json` (resolvido pelo Unity).
- `Packages/com.kitwright.unity.mcp/` (pacote incorporado e correcao descrita acima).
- `.codex/config.toml` (configuracao restrita ao projeto).
- `.codex/skills/unity-mcp-workflow/SKILL.md`.
- `.kitwright/skills/manifest.json`.
- `AGENTS.md` (orientacoes da skill geradas pelo pacote).
- `UserSettings/KitWrightMcpSettings.json` (configuracao local gerada pelo pacote).
- `KITWRIGHT_INSTALLATION.md` (este registro).

O arquivo global de configuracao do Codex nao foi alterado. Os arquivos em UserSettings e a configuracao local com porta/identidade dependem desta maquina e deste caminho de projeto.
