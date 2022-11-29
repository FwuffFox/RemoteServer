export function scrollToEnd() {
    window.scrollTo(0, document.body.scrollHeight);
}

export function setEnterToMsgBox()
{
    $("#msg-input").onkeydown(function (e) {
        if(e.which === 13 && !e.shiftKey) {
            e.preventDefault();

            $(this).closest("form").submit();
        }
    });
}

function $(x) {
    return document.getElementById(x);
}