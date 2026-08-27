

window.layoutHelper = {
    alterarBackground: function (cor) {
        console.log("alterarBackground", cor);
        document.body.style.backgroundColor = cor;
    },

    restaurarBackground: function () {
        console.log("restaurarBackground");
        document.body.style.backgroundColor = "";
    }
};

// Rola um item de acordeão (sub-validação, item de "Preencher FPD", etc.)
// pra dentro da área visível quando ele é expandido — sem isso, um item
// aberto perto do fim da lista fica com a caixa de comentário escondida
// atrás do rodapé fixo do dialog (Cancelar/Aprovar/Etapa seguinte), e o
// usuário precisa rolar manualmente pra achar o campo de texto.
// "nearest": não mexe se já estiver visível; só rola o mínimo necessário.
window.scrollSubItemIntoView = function (elementId) {
    var el = document.getElementById(elementId);
    if (!el) return;
    // "auto" (instantâneo) em vez de "smooth": scroll suave via
    // scrollIntoView não é confiável em todo navegador/contexto (algumas
    // combinações de aba sem foco, automação, ou configuração do usuário
    // simplesmente ignoram o smooth-scroll sem erro nenhum — o item fica
    // "preso" fora de vista e ninguém percebe o motivo). Instantâneo sempre
    // funciona, e a diferença visual aqui é imperceptível.
    el.scrollIntoView({ behavior: "auto", block: "nearest" });
};