/**
 * Utilitário para converter propriedades de objetos entre diferentes formatos de casing
 */

/**
 * Converte todas as propriedades do objeto de snake/camel case (frontend) para Pascal Case (backend)
 * Por exemplo, transforma { id: 1, nome: 'João', data_nascimento: '2000-01-01' } em { Id: 1, Nome: 'João', DataNascimento: '2000-01-01' }
 */
export const toPascalCase = (obj) => {
    if (!obj || typeof obj !== 'object') return obj;
    
    if (Array.isArray(obj)) {
        return obj.map(item => toPascalCase(item));
    }
    
    const result = {};
    
    Object.keys(obj).forEach(key => {
        if (obj[key] === null || obj[key] === undefined) {
            result[key.charAt(0).toUpperCase() + key.slice(1)] = obj[key];
            return;
        }
        
        if (typeof obj[key] === 'object') {
            result[key.charAt(0).toUpperCase() + key.slice(1)] = toPascalCase(obj[key]);
            return;
        }
        
        result[key.charAt(0).toUpperCase() + key.slice(1)] = obj[key];
    });
    
    return result;
};

/**
 * Converte todas as propriedades do objeto de Pascal Case (backend) para camelCase (frontend)
 * Por exemplo, transforma { Id: 1, Nome: 'João', DataNascimento: '2000-01-01' } em { id: 1, nome: 'João', dataNascimento: '2000-01-01' }
 */
export const toCamelCase = (obj) => {
    if (!obj || typeof obj !== 'object') return obj;
    
    if (Array.isArray(obj)) {
        return obj.map(item => toCamelCase(item));
    }
    
    const result = {};
    
    Object.keys(obj).forEach(key => {
        if (obj[key] === null || obj[key] === undefined) {
            result[key.charAt(0).toLowerCase() + key.slice(1)] = obj[key];
            return;
        }
        
        if (typeof obj[key] === 'object' && !Array.isArray(obj[key]) && !(obj[key] instanceof Date)) {
            result[key.charAt(0).toLowerCase() + key.slice(1)] = toCamelCase(obj[key]);
            return;
        }

        if (Array.isArray(obj[key])) {
            result[key.charAt(0).toLowerCase() + key.slice(1)] = obj[key].map(item => 
                typeof item === 'object' ? toCamelCase(item) : item
            );
            return;
        }
        
        result[key.charAt(0).toLowerCase() + key.slice(1)] = obj[key];
    });
    
    return result;
};
