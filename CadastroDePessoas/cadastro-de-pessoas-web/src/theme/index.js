import { extendTheme } from '@chakra-ui/react';

const colors = {
    brand: {
        50: '#e0f5ff',
        100: '#b8dcff',
        200: '#8ec2ff',
        300: '#63a9ff',
        400: '#3990ff',
        500: '#1e77f3',
        600: '#1260cc',
        700: '#0549a6',
        800: '#003380',
        900: '#001e59',
    },
    accent: {
        50: '#e6fffa',
        100: '#b3f5ec',
        200: '#81ebe1',
        300: '#4edfd8',
        400: '#2bd4ce',
        500: '#00bab3',
        600: '#00938d',
        700: '#006e68',
        800: '#004a45',
        900: '#002522',
    },
};

const fonts = {
    heading: "'Inter', sans-serif",
    body: "'Inter', sans-serif",
};

const theme = extendTheme({
    colors,
    fonts,
    components: {
        Button: {
            baseStyle: {
                fontWeight: 'bold',
                borderRadius: 'md',
            },
            variants: {
                primary: {
                    bg: 'brand.500',
                    color: 'white',
                    _hover: {
                        bg: 'brand.600',
                    },
                },
                secondary: {
                    bg: 'accent.500',
                    color: 'white',
                    _hover: {
                        bg: 'accent.600',
                    },
                },
                outline: {
                    border: '2px solid',
                    borderColor: 'brand.500',
                    color: 'brand.500',
                },
                danger: {
                    bg: 'red.500',
                    color: 'white',
                    _hover: {
                        bg: 'red.600',
                    },
                },
            },
        },
        Card: {
            baseStyle: {
                p: '6',
                bg: 'white',
                rounded: 'lg',
                boxShadow: 'md',
            },
        },
    },
    styles: {
        global: {
            body: {
                bg: 'gray.50',
                color: 'gray.800',
            },
        },
    },
});

export default theme;