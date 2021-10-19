import CryptoJS from 'crypto-js';

export default class RijndaelService {
    static encrypt(plainText, rijndaelKey, rijndaelIV) {
        const cipherText = CryptoJS.AES.encrypt(
            plainText,
            CryptoJS.enc.Utf8.parse(rijndaelKey),
            {
                keySize: 128 / 8,
                iv: CryptoJS.enc.Utf8.parse(rijndaelIV),
                mode: CryptoJS.mode.CBC,
                padding: CryptoJS.pad.Pkcs7
            });
        return cipherText.toString();
    }

    static decrypt(cipherText, rijndaelKey, rijndaelIV) {
        const plainText = CryptoJS.AES.decrypt(
            cipherText,
            CryptoJS.enc.Utf8.parse(rijndaelKey),
            {
                keySize: 128 / 8,
                iv: CryptoJS.enc.Utf8.parse(rijndaelIV),
                mode: CryptoJS.mode.CBC,
                padding: CryptoJS.pad.Pkcs7
            });
        return plainText.toString(CryptoJS.enc.Utf8);
    }
}