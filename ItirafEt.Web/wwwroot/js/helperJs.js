window.triggerClick = (wrapper) => {
    const input = wrapper.querySelector('input[type="file"]');
    if (input) input.click();
};
