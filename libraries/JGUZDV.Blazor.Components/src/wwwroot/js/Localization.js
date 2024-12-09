export function readLanguageCookieValue(cookieName) {
    const name = cookieName + "=";
    const decodedCookie = decodeURIComponent(document.cookie);
    const languageCookie = decodedCookie.split(';')
        .find(v => v.indexOf(name) === 0);

    return languageCookie ? languageCookie.substring(name.length) : null;
}

export function setOrReplaceLanguageCookieValue(cookieName, value) {
    document.cookie = `${cookieName}=${value}`;
}