export const scrollToTop = (element, scrollDuration) => {
    const scrollStep = -element.scrollTop / (scrollDuration / 15);
    requestAnimationFrame(() => {
        let scrollInterval = null;
        scrollInterval = setInterval(() => {
            if (element.scrollTop !== 0) {
                element.scrollTop += scrollStep;
            } else {
                clearInterval(scrollInterval);
            }
        }, 15);
    });
};

const functions = {
    scrollToTop
};

export default functions;